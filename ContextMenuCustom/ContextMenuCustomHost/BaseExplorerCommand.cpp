#include "pch.h"
#include "BaseExplorerCommand.h"

const EXPCMDFLAGS BaseExplorerCommand::Flags() { return ECF_DEFAULT; }
const EXPCMDSTATE BaseExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }
const  wchar_t* BaseExplorerCommand::GetIconId() { return L",-101"; }

IFACEMETHODIMP BaseExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	return SHStrDupW(L"Open With", name);
}

IFACEMETHODIMP BaseExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon)
{
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
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.Get(), &parent));
	}

	wil::unique_cotaskmem_string path = GetPath(selection);

	if (path.is_valid()) {
		MessageBox(parent, path.get(), L"ContextMenu", MB_OK);
	}

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