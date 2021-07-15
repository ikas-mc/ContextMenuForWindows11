#include "pch.h"
#include "BaseExplorerCommand.h"

const  wchar_t* BaseExplorerCommand::Title() { return L""; }
const EXPCMDFLAGS BaseExplorerCommand::Flags() { return ECF_DEFAULT; }
const EXPCMDSTATE BaseExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }
const  wchar_t* BaseExplorerCommand::GetIconId() { return L",-101"; }

IFACEMETHODIMP BaseExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	auto title = wil::make_cotaskmem_string_nothrow(Title());
	RETURN_IF_NULL_ALLOC(title);
	*name = title.release();
	return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon)
{
	UNREFERENCED_PARAMETER(items);
	*icon = nullptr;
	std::filesystem::path modulePath{ wil::GetModuleFileNameW<std::wstring>(wil::GetModuleInstanceHandle()) };
	const auto iconPath{ modulePath.wstring() + GetIconId() };
	return SHStrDupW(iconPath.c_str(), icon);
}

IFACEMETHODIMP BaseExplorerCommand::GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip)
{
	*infoTip = nullptr;
	return E_NOTIMPL;
}

IFACEMETHODIMP BaseExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName)
{
	*guidCommandName = GUID_NULL;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
{
	*cmdState = State(selection);
	return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		ComPtr<IOleWindow> oleWindow;
		RETURN_IF_FAILED(m_site.As(&oleWindow));
		RETURN_IF_FAILED(oleWindow->GetWindow(&parent));
	}

	std::wostringstream title;
	title << Title();

	if (selection)
	{
		DWORD count;
		RETURN_IF_FAILED(selection->GetCount(&count));
		if (count > 0) {
			IShellItem* item;
			auto hr = selection->GetItemAt(0, &item);
			if (SUCCEEDED(hr)) {
				LPOLESTR path = NULL;
				hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
				if (SUCCEEDED(hr))
				{
					title << L" Item:" << path;
				}
				item->Release();
			}
		}
	}
	MessageBox(parent, title.str().c_str(), L"ContextMenu", MB_OK);
	return S_OK;
}
CATCH_RETURN();

IFACEMETHODIMP BaseExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags)
{
	*flags = Flags();
	return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	return E_NOTIMPL;
}

IFACEMETHODIMP BaseExplorerCommand::SetSite(_In_ IUnknown* site) noexcept
{
	m_site = site;
	return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept
{
	return m_site.CopyTo(riid, site);
}