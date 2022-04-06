#include "pch.h"
#include "CustomExplorerCommand.h"
#include "CustomSubExplorerCommand.h"
#include "CustomExplorerCommandEnum.h"
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

using namespace winrt::Windows::Storage;
using namespace winrt::Windows::Data::Json;
using namespace std::filesystem;

CustomExplorerCommand::CustomExplorerCommand(){
}

const EXPCMDFLAGS CustomExplorerCommand::Flags() { return ECF_HASSUBCOMMANDS; }

IFACEMETHODIMP CustomExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	winrt::hstring title = winrt::unbox_value_or<winrt::hstring>(winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().Lookup(L"Custom_Menu_Name"), L"Open With");
	return SHStrDupW(title.data(), name);
}

IFACEMETHODIMP CustomExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName)
{
	*guidCommandName = __uuidof(this);
	return S_OK;
}

const  wchar_t* CustomExplorerCommand::GetIconId()
{
	DWORD value = 0;
	DWORD size = sizeof(value);
	auto result = SHRegGetValueW(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", L"AppsUseLightTheme", SRRF_RT_DWORD, NULL, &value, &size);
	if (result == ERROR_SUCCESS && !!value) {
		return L",-103";
	}
	else {
		return L",-101";
	}
}

IFACEMETHODIMP CustomExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	HRESULT hr;

	if (m_site)
	{
		//hidden menu on the classic context menu.
		Microsoft::WRL::ComPtr<IOleWindow> oleWindow;
		m_site.As(&oleWindow);
		if (oleWindow)
		{
			// fix right click on explorer left tree view 
			//https://github.com/TortoiseGit/TortoiseGit/blob/master/src/TortoiseShell/ContextMenu.cpp
			HWND hWnd = nullptr;
			oleWindow->GetWindow(&hWnd);
			TCHAR szWndClassName[MAX_PATH] = { 0 };
			GetClassName(hWnd, szWndClassName, MAX_PATH);
			// window class name: "NamespaceTreeControl"
			if (StrCmp(szWndClassName, L"NamespaceTreeControl"))
			{
				*cmdState = ECS_HIDDEN;
			    return S_OK;
			}
		}
	}

	if (okToBeSlow)
	{
		if (selection) {
			DWORD count;
			selection->GetCount(&count);
			if (count > 1) {
				std::wstring currentPath;
				ReadCommands(true, currentPath);
			}
			else {
				auto currentPath = PathHelper::getPath(selection);
				ReadCommands(false, currentPath);
			}
		}
		else {
			std::wstring currentPath;
			//fix right click on desktop 
			//https://github.com/microsoft/terminal/blob/main/src/cascadia/ShellExtension/OpenTerminalHere.cpp
			auto hwnd = ::GetForegroundWindow();
			if (hwnd)
			{
				TCHAR szName[MAX_PATH] = { 0 };
				::GetClassName(hwnd, szName, MAX_PATH);
				if (0 == StrCmp(szName, L"WorkerW") ||
					0 == StrCmp(szName, L"Progman"))
				{
					//special folder: desktop
					hr = ::SHGetFolderPath(NULL, CSIDL_DESKTOP, NULL, SHGFP_TYPE_CURRENT, szName);
					if (SUCCEEDED(hr))
					{
						currentPath= szName;
					}
				}
			}
		
			ReadCommands(false, currentPath);
		}

		if (m_commands.size() == 0) {
			*cmdState = ECS_HIDDEN;
		}
		else {
			*cmdState = ECS_ENABLED;
		}
	}
	else
	{
		*cmdState = ECS_DISABLED;
		hr = E_PENDING;
	}

	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommand::EnumSubCommands(__RPC__deref_out_opt IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	auto customCommands = Make<CustomExplorerCommandEnum>(m_commands);
	return customCommands->QueryInterface(IID_PPV_ARGS(enumCommands));
}

void CustomExplorerCommand::ReadCommands(bool multipeFiles,const std::wstring& currentPath)
{
	auto menus = winrt::Windows::Storage::ApplicationData::Current().LocalSettings().CreateContainer(L"menus", ApplicationDataCreateDisposition::Always).Values();
	if (menus.Size() > 0) {
		std::wstring ext;
		bool isDirectory = true; //TODO current_path may be empty when right click on desktop.  set directory as default?
		if (!multipeFiles) {
			PathHelper::getExt(currentPath, isDirectory, ext);
		}

		auto current = menus.begin();
		do {
			if (current.HasCurrent()) {
				auto conent=winrt::unbox_value_or<winrt::hstring>(current.Current().Value(), L"");
				if (conent.size() > 0) {
					const auto command = Make<CustomSubExplorerCommand>(conent);
					if (command->Accept(multipeFiles,isDirectory, ext)) {
						m_commands.push_back(command);
					}
				}
			}
		} while (current.MoveNext());
	}
	else {
		auto localFolder = ApplicationData::Current().LocalFolder().Path();
		concurrency::create_task([&]
			{
				path folder{ localFolder.c_str() };
				folder /= "custom_commands";
				if (exists(folder) && is_directory(folder)) {
					std::wstring ext;
					bool isDirectory = true; //TODO current_path may be empty when right click on desktop.  set directory as default?
					if (!multipeFiles) {
						PathHelper::getExt(currentPath, isDirectory, ext);
					}

					for (auto& file : directory_iterator{ folder })
					{
						std::ifstream fs{ file.path() };
						std::stringstream buffer;
						buffer << fs.rdbuf();//TODO 
						auto content = winrt::to_hstring(buffer.str());
						auto command = Make<CustomSubExplorerCommand>(content);
						if (command->Accept(multipeFiles,isDirectory, ext)) {
							m_commands.push_back(command);
						}
					}
				}
			}).wait();
	}
}
