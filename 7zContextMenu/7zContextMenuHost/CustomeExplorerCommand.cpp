#include "pch.h"
#include "CustomeExplorerCommand.h"
#include <winrt\base.h>
#include <winrt\Windows.Storage.h>
#include <winrt\Windows.Data.Json.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>
#include <fstream>
#include <ppltasks.h>
#pragma comment(lib, "windowsapp")

const  wchar_t* CustomeExplorerCommand::Title() { return L"Custome"; };
const EXPCMDSTATE CustomeExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
const EXPCMDFLAGS CustomeExplorerCommand::Flags() { return ECF_HASSUBCOMMANDS; }



IFACEMETHODIMP CustomeExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	auto e = Make<CustomeCommands>();
	e->ReadCommands();
	return e->QueryInterface(IID_PPV_ARGS(enumCommands));
}




CustomeCommands::CustomeCommands() {

}


void CustomeCommands::ReadCommands()
{
	using namespace winrt::Windows::Storage;
	using namespace winrt::Windows::Data::Json;
	using namespace std::filesystem;

	auto localFolder = ApplicationData::Current().LocalFolder().Path();
	path localFolderPath{ localFolder.c_str() };
	localFolderPath /= "custom_commands";

	auto task = concurrency::create_task([&]
		{
			if (exists(localFolderPath) && is_directory(localFolderPath)) {
				auto configsFolder = StorageFolder::GetFolderFromPathAsync(localFolderPath.c_str()).get();
				auto files = configsFolder.GetFilesAsync().get();
				for (auto configFile : files) {
					const auto command = Make<CustomeExplorerItemCommand>();
					const auto content = FileIO::ReadTextAsync(configFile).get();
					JsonObject result;
					auto success = JsonObject::TryParse(content, result);
					if (success) { 
						command->_title = result.GetNamedString(L"title");
						command->_exe = result.GetNamedString(L"exe");
						command->_param = result.GetNamedString(L"param");
					}
					m_commands.push_back(command);
				}
			}
		});
	task.wait();
	//winrt::hstring DisplayName = winrt::unbox_value_or<winrt::hstring>(winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().Lookup(L"GlobalizationStringForContextMenu"), DefaultDisplayName);
	m_current = m_commands.cbegin();
}
IFACEMETHODIMP CustomeCommands::Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
{
	ULONG fetched{ 0 };
	wil::assign_to_opt_param(pceltFetched, 0ul);

	for (ULONG i = 0; (i < celt) && (m_current != m_commands.cend()); i++)
	{
		m_current->CopyTo(&apUICommand[0]);
		m_current++;
		fetched++;
	}

	wil::assign_to_opt_param(pceltFetched, fetched);
	return (fetched == celt) ? S_OK : S_FALSE;
}
IFACEMETHODIMP CustomeCommands::Skip(ULONG /*celt*/) { return E_NOTIMPL; }
IFACEMETHODIMP CustomeCommands::Reset()
{
	m_current = m_commands.cbegin();
	return S_OK;
}
IFACEMETHODIMP CustomeCommands::Clone(__deref_out IEnumExplorerCommand** ppenum) { *ppenum = nullptr; return E_NOTIMPL; }



CustomeExplorerItemCommand::CustomeExplorerItemCommand() {

}
const wchar_t* CustomeExplorerItemCommand::Title() {
	if (_title.empty()) {
		return L"CustomeExplorerItemCommand1";
	}
	else {
		return _title.c_str();
	}
}
const EXPCMDSTATE CustomeExplorerItemCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }


static std::wstring string_replace_all(std::wstring src, std::wstring const& target, std::wstring const& repl) {

	if (target.length() == 0) {
		// searching for a match to the empty string will result in
		//  an infinite loop
		//  it might make sense to throw an exception for this case
		return src;
	}

	if (src.length() == 0) {
		return src;  // nothing to match against
	}

	size_t idx = 0;

	for (;;) {
		idx = src.find(target, idx);
		if (idx == std::wstring::npos)  break;

		src.replace(idx, target.length(), repl);
		idx += repl.length();
	}

	return src;
}

IFACEMETHODIMP CustomeExplorerItemCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		ComPtr<IOleWindow> oleWindow;
		RETURN_IF_FAILED(m_site.As(&oleWindow));
		RETURN_IF_FAILED(oleWindow->GetWindow(&parent));
	}

	if (selection)
	{
		DWORD count;
		RETURN_IF_FAILED(selection->GetCount(&count));
		if (count > 0) {
			IShellItem* item;
			auto hr = selection->GetItemAt(0, &item);
			if (SUCCEEDED(hr)) {
				LPOLESTR path = nullptr;
				hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
				if (SUCCEEDED(hr))
				{
				    auto param=	string_replace_all(_param, L"{path}", path);
					ShellExecute(nullptr, L"open",_exe.c_str(), param.c_str(), nullptr, SW_HIDE);
				}
				item->Release();
			}
		}
	}
	else
	{
		MessageBox(parent, L"(no selected items)", L"vscode menu", MB_OK);
	}

	return S_OK;
}
CATCH_RETURN();