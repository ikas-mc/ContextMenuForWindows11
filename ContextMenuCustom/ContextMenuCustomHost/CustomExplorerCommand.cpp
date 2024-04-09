#include "pch.h"
#include "CustomExplorerCommand.h"
#include "CustomExplorerCommandEnum.h"
#include "CustomSubExplorerCommand.h"
#include <winrt/base.h>
#include <winrt/Windows.Storage.h>
#include <winrt/Windows.Data.Json.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <fstream>
#include <ppltasks.h>
#include <shlwapi.h>
#include "PathHelper.hpp"
#include <ShlObj.h>
#include <wil/com.h>

using namespace winrt::Windows::Storage;
using namespace winrt::Windows::Data::Json;
using namespace std::filesystem;

CustomExplorerCommand::CustomExplorerCommand() = default;

IFACEMETHODIMP CustomExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags) {
	*flags = ECF_HASSUBCOMMANDS;

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon) {
	*icon = nullptr;

	if (m_commands.size() == 1) {
		return m_commands.at(0)->GetIcon(items, icon);
	}
	return BaseExplorerCommand::GetIcon(items, icon);
}

IFACEMETHODIMP CustomExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) {
	*name = nullptr;

	if (m_commands.size() == 1) {
		return m_commands.at(0)->GetTitle(items, name);
	}

	const auto title = winrt::unbox_value_or<winrt::hstring>(ApplicationData::Current().LocalSettings().Values().Lookup(L"Custom_Menu_Name"), L"Open With");
	auto titleStr = wil::make_cotaskmem_string_nothrow(title.data());
	RETURN_IF_NULL_ALLOC(titleStr);
	*name = titleStr.release();
	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName) {
	*guidCommandName = __uuidof(this);

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState okToBeSlow={}",okToBeSlow).c_str());
	if (!okToBeSlow) {
		*cmdState = ECS_DISABLED;
		return E_PENDING;
	}

	if (m_site) {
		// hidden menu on the classic context menu.
		// classic menu provides an IOleWindow
		wil::com_ptr_nothrow<IOleWindow> oleWindow;
		m_site.query_to(IID_PPV_ARGS(oleWindow.put()));
		if (oleWindow) {
			OutputDebugStringW(L"CustomExplorerCommand::GetState loading from classic context menu");
			// fix right click on explorer left tree view
			// https://github.com/TortoiseGit/TortoiseGit/blob/master/src/TortoiseShell/ContextMenu.cpp
			HWND hWnd = nullptr;
			if (SUCCEEDED(oleWindow->GetWindow (&hWnd))) {
				wchar_t szWndClassName[MAX_PATH] = {0};
				GetClassName(hWnd, szWndClassName, _countof(szWndClassName));
				// window class name: "NamespaceTreeControl"
				OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState loading from classic context menu,source={}", szWndClassName).c_str());
				if (wcscmp(szWndClassName, L"NamespaceTreeControl") != 0) {
					*cmdState = ECS_HIDDEN;
					return S_OK;
				}
			}
		}
	}

	DWORD count = 0;
	if (selection) {
		selection->GetCount(&count);
	}
	OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState selection count:{}",count).c_str());

	//theme type
	DWORD themeValue = 0;
	DWORD themeValueSize = sizeof(themeValue);
	const std::filesystem::path modulePath{ wil::GetModuleFileNameW<std::wstring>(wil::GetModuleInstanceHandle()) };
	const auto result = SHRegGetValueW(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", L"AppsUseLightTheme", SRRF_RT_DWORD, nullptr, &themeValue, &themeValueSize);
	if (result == ERROR_SUCCESS) {
		m_theme_type = themeValue == 0 ? ThemeType::Dark : ThemeType::Light;
	}
	OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState m_theme_type={}", static_cast<int>(m_theme_type)).c_str());

	//
	if (count > 1) {
		const std::wstring currentPath;
		ReadCommands(true,false,false, currentPath);
	} else if (count == 1) {
		const auto currentPath = PathHelper::getPath(selection);
		ReadCommands(false,false,false, currentPath);
	} else {
		wil::com_ptr_nothrow<IShellItem> psi;
		FindLocationFromSite(psi.put());
		if (psi) {
			const auto currentPath = PathHelper::getPath(psi.get());
			OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState background={}",currentPath).c_str());

			//TODO 
			bool isDesktop = false;
			wchar_t desktopPath[MAX_PATH] = {0};
			if (SUCCEEDED(::SHGetFolderPath(NULL, CSIDL_DESKTOP, NULL, SHGFP_TYPE_CURRENT, desktopPath))) {
				isDesktop = wcscmp(currentPath.c_str(), desktopPath) == 0;
				OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState desktopPath={}",desktopPath).c_str());
				// if(isDesktop) {
				// 	if (const HWND foregroundWindow = GetForegroundWindow()) {
				// 		wchar_t className[256];
				// 		GetClassName(foregroundWindow, className, _countof(className)); 
				// 		OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState GetForegroundWindow={}",m_commands.size(),className).c_str());
				// 		isDesktop =wcscmp(className, L"Progman") == 0 || wcscmp(className, L"WorkerW") == 0;
				// 	}
				// }
			}
			
			ReadCommands(false, true, isDesktop, currentPath);
		}
	}

	OutputDebugStringW(std::format(L"CustomExplorerCommand::GetState m_commands={}",m_commands.size()).c_str());
	if (m_commands.empty()) {
		*cmdState = ECS_HIDDEN;
	} else {
		*cmdState = ECS_ENABLED;
	}

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::EnumSubCommands(__RPC__deref_out_opt IEnumExplorerCommand** enumCommands) {
	wil::assign_null_to_opt_param(enumCommands);

	if (m_commands.size() == 1) {
		return E_NOTIMPL;
	}
	const auto customCommands = Make<CustomExplorerCommandEnum>(m_commands);
	return customCommands->QueryInterface(IID_PPV_ARGS(enumCommands));
}

void CustomExplorerCommand::ReadCommands(bool multipleFiles,bool isBackground,bool isDesktop, const std::wstring& currentPath) {
	std::wstring ext;
	std::wstring name;
	bool isDirectory = false; // TODO current_path may be empty when right click on desktop.  set directory as default?
	if (!multipleFiles) {
		PathHelper::getExt(currentPath, isDirectory, name, ext);
	}
	
	FileType fileType;
	if(isDesktop) {
		fileType=FileType::Desktop;
	}else if(isBackground) {
		fileType=FileType::Background;
	}else if(isDirectory) {
		fileType=FileType::Directory;
	}else {
		fileType=FileType::File;
	}

	OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands multipleFiles={},isBackground={},isDesktop={},fileType={},currentPath={}",multipleFiles,isBackground,isDesktop, static_cast<int>(fileType),currentPath).c_str());

	const auto menus = ApplicationData::Current().LocalSettings().CreateContainer(L"menus", ApplicationDataCreateDisposition::Always).Values();
	if (menus.Size() > 0) {
		OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands useCache={},menus={}",true,menus.Size()).c_str());

		const auto current = menus.begin();
		do {
			if (current.HasCurrent()) {
				if (auto content = winrt::unbox_value_or<winrt::hstring>(current.Current().Value(), L""); !content.empty()) {
					const auto command = Make<CustomSubExplorerCommand>(content,m_theme_type);
					if (command->Accept(multipleFiles, fileType, name, ext)) {
						m_commands.push_back(command);
					}
				}
			}
		} while (current.MoveNext());
	} else {
		OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands useCache={}",false).c_str());
		const auto localFolder = ApplicationData::Current().LocalFolder().Path();
		concurrency::create_task([&] {
			path folder{localFolder.c_str()};
			folder /= "custom_commands";
			if (exists(folder) && is_directory(folder)) {
				
					for (auto& file : directory_iterator{ folder }) {
						OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands useCache={},file={}", false, file.path().c_str()).c_str());
						
						try
						{
							std::ifstream fs{ file.path() };
							std::stringstream buffer;
							buffer << fs.rdbuf(); //TODO 
							auto content = winrt::to_hstring(buffer.str());
							//OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands useCache={},file={},content={}", false, file.path().c_str(), content).c_str());

							//std::ifstream fs(file.path(), std::ios::binary);
							//std::string contentString{ std::istreambuf_iterator<char>{ fs },  std::istreambuf_iterator<char>{} };
							//auto content = winrt::to_hstring(contentString);

							auto command = Make<CustomSubExplorerCommand>(content, m_theme_type);
							if (command->Accept(multipleFiles, fileType, name, ext)) {
								m_commands.push_back(command);
							}
						}
						catch (winrt::hresult_error const& e)
						{
							OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands error useCache={},file={},e={}", false, file.path().c_str(), e.message()).c_str());
						}
						
					}
			}
		}).wait();
	}

	if (m_commands.size() > 1) {
		std::ranges::sort(m_commands, [](auto&& l, auto&& r) {
			return l->m_index < r->m_index;
		});
	}

	OutputDebugStringW(std::format(L"CustomExplorerCommand::ReadCommands m_commands={}",m_commands.size()).c_str());
}

IFACEMETHODIMP CustomExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx* ctx) noexcept try {
	if (m_commands.size() == 1) {
		return m_commands.at(0)->Invoke(selection, ctx);
	}

	return S_OK;
}CATCH_RETURN();


HRESULT CustomExplorerCommand::FindLocationFromSite(IShellItem** location) const noexcept {
	wil::assign_null_to_opt_param(location);

	if (!m_site) {
		return S_FALSE;
	}

	//https://github.com/microsoft/terminal/blame/main/src/cascadia/ShellExtension/OpenTerminalHere.cpp#L157
	wil::com_ptr_nothrow<IServiceProvider> serviceProvider;
	RETURN_IF_FAILED(m_site.query_to(serviceProvider.put()));
	wil::com_ptr_nothrow<IFolderView> folderView;
	RETURN_IF_FAILED(serviceProvider->QueryService(SID_SFolderView, IID_PPV_ARGS(folderView.put())));
	RETURN_IF_FAILED(folderView->GetFolder(IID_PPV_ARGS(location)));
	return S_OK;
}

bool CustomExplorerCommand::IsShiftPressed()
{
	short shift = 0;
	shift = GetAsyncKeyState(VK_SHIFT);
	return  shift < 0;
}
