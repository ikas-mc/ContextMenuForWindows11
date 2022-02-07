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
	auto result = SHRegGetValueW(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", L"SystemUsesLightTheme", SRRF_RT_DWORD, NULL, &value, &size);
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
		Microsoft::WRL::ComPtr<IOleWindow> oleWindow;
		m_site.As(&oleWindow);
		if (oleWindow)
		{
			*cmdState = ECS_HIDDEN;
			return S_OK;
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
			//right click on desktop. 
			//TODO 
			std::wstring currentPath;
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
