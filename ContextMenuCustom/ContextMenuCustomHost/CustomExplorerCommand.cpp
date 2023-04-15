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

CustomExplorerCommand::CustomExplorerCommand()
{
}

IFACEMETHODIMP CustomExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS *flags)
{
	*flags = ECF_HASSUBCOMMANDS;

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::GetIcon(_In_opt_ IShellItemArray *items, _Outptr_result_nullonfailure_ PWSTR *icon)
{
	*icon = nullptr;

	if (m_commands.size() == 1)
	{
		return m_commands.at(0)->GetIcon(items, icon);
	}
	else
	{
		return BaseExplorerCommand::GetIcon(items, icon);
	}
}

IFACEMETHODIMP CustomExplorerCommand::GetTitle(_In_opt_ IShellItemArray *items, _Outptr_result_nullonfailure_ PWSTR *name)
{
	*name = nullptr;

	if (m_commands.size() == 1)
	{
		return m_commands.at(0)->GetTitle(items, name);
	}

	winrt::hstring title = winrt::unbox_value_or<winrt::hstring>(winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().Lookup(L"Custom_Menu_Name"), L"Open With");
	return SHStrDupW(title.data(), name);
}

IFACEMETHODIMP CustomExplorerCommand::GetCanonicalName(_Out_ GUID *guidCommandName)
{
	*guidCommandName = __uuidof(this);

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::GetState(_In_opt_ IShellItemArray *selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE *cmdState)
{

	if (m_site)
	{
		// hidden menu on the classic context menu.
		// classic menu provides an IOleWindow
		wil::com_ptr_nothrow<IOleWindow> oleWindow;
		m_site.query_to(IID_PPV_ARGS(oleWindow.put()));
		if (oleWindow)
		{
			// fix right click on explorer left tree view
			// https://github.com/TortoiseGit/TortoiseGit/blob/master/src/TortoiseShell/ContextMenu.cpp
			HWND hWnd = nullptr;
			oleWindow->GetWindow(&hWnd);
			wchar_t szWndClassName[MAX_PATH] = { 0 };
			GetClassName(hWnd, szWndClassName, _countof(szWndClassName));
			// window class name: "NamespaceTreeControl"
			if (wcscmp(szWndClassName, L"NamespaceTreeControl") != 0)
			{
				*cmdState = ECS_HIDDEN;
				return S_OK;
			}
		}
	}

	if (!okToBeSlow)
	{
		*cmdState = ECS_DISABLED;
		return E_PENDING;
	}

	DWORD count =0;

	if (selection)
	{
		selection->GetCount(&count);
	}

	if (count > 1)
	{
		std::wstring currentPath;
		ReadCommands(true, currentPath);
	}
	else if (count == 1)
	{
		auto currentPath = PathHelper::getPath(selection);
		ReadCommands(false, currentPath);
	}
	else
	{
		wil::com_ptr_nothrow<IShellItem> psi;
		FindLocationFromSite(psi.put());
		if (psi)
		{
			auto currentPath = PathHelper::getPath(psi.get());
			ReadCommands(false, currentPath);
		}
	}

	if (m_commands.empty())
	{
		*cmdState = ECS_HIDDEN;
	}
	else
	{
		*cmdState = ECS_ENABLED;
	}

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::EnumSubCommands(__RPC__deref_out_opt IEnumExplorerCommand **enumCommands)
{
	wil::assign_null_to_opt_param(enumCommands);

	if (m_commands.size() == 1)
	{
		return E_NOTIMPL;
	}
	else
	{
		auto customCommands = Make<CustomExplorerCommandEnum>(m_commands);
		return customCommands->QueryInterface(IID_PPV_ARGS(enumCommands));
	}
}

void CustomExplorerCommand::ReadCommands(bool multipleFiles, const std::wstring &currentPath)
{
	auto menus = winrt::Windows::Storage::ApplicationData::Current().LocalSettings().CreateContainer(L"menus", ApplicationDataCreateDisposition::Always).Values();
	if (menus.Size() > 0)
	{
		std::wstring ext;
		std::wstring name;
		bool isDirectory = true; // TODO current_path may be empty when right click on desktop.  set directory as default?
		if (!multipleFiles)
		{
			PathHelper::getExt(currentPath, isDirectory, name, ext);
		}

		auto current = menus.begin();
		do
		{
			if (current.HasCurrent())
			{
				auto content = winrt::unbox_value_or<winrt::hstring>(current.Current().Value(), L"");
				
				if (!content.empty())
				{
					const auto command = Make<CustomSubExplorerCommand>(content);
					if (command->Accept(multipleFiles, isDirectory, name, ext))
					{
						m_commands.push_back(command);
					}
				}
			}
		} while (current.MoveNext());
	}
	else
	{
		auto localFolder = ApplicationData::Current().LocalFolder().Path();
		concurrency::create_task([&] {
			path folder{ localFolder.c_str() };
			folder /= "custom_commands";
			if (exists(folder) && is_directory(folder)) {
				std::wstring ext;
				std::wstring name;
				bool isDirectory = true; //TODO current_path may be empty when right click on desktop.  set directory as default?
				if (!multipleFiles) {
					PathHelper::getExt(currentPath, isDirectory, name, ext);
				}

				for (auto& file : directory_iterator{ folder })
				{
					std::ifstream fs{ file.path() };
					std::stringstream buffer;
					buffer << fs.rdbuf();//TODO 
					auto content = winrt::to_hstring(buffer.str());
					auto command = Make<CustomSubExplorerCommand>(content);
					if (command->Accept(multipleFiles, isDirectory, name, ext)) {
						m_commands.push_back(command);
					}
				}
			}
		}).wait();
	}

	if (m_commands.size() > 1)
	{
		std::sort(m_commands.begin(), m_commands.end(), [](auto &&l, auto &&r)
				  { return l->m_index < r->m_index; });
	}
}

IFACEMETHODIMP CustomExplorerCommand::Invoke(_In_opt_ IShellItemArray *selection, _In_opt_ IBindCtx *ctx) noexcept
try
{

	if (m_commands.size() == 1)
	{
		return m_commands.at(0)->Invoke(selection, ctx);
	}

	return S_OK;
}
CATCH_RETURN();


HRESULT CustomExplorerCommand::FindLocationFromSite(IShellItem** location) const noexcept
{
	wil::assign_null_to_opt_param(location);

	if (!m_site)
	{
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

