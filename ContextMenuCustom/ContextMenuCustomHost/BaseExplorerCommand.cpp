#include "pch.h"
#include "BaseExplorerCommand.h"


IFACEMETHODIMP BaseExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	return SHStrDupW(L"Open With", name);
}

IFACEMETHODIMP BaseExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon)
{
	*icon = nullptr;
	std::filesystem::path modulePath{ wil::GetModuleFileNameW<std::wstring>(wil::GetModuleInstanceHandle()) };

	DWORD value = 0;
	DWORD size = sizeof(value);
	auto result = SHRegGetValueW(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", L"AppsUseLightTheme", SRRF_RT_DWORD, NULL, &value, &size);
	if (result == ERROR_SUCCESS && !!value) {
		const auto iconPath{ modulePath.wstring() + L",-103" };
		return SHStrDupW(iconPath.c_str(), icon);
	}
	else {
		const auto iconPath{ modulePath.wstring() + L",-101" };
		return SHStrDupW(iconPath.c_str(), icon);
	}
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
	*cmdState = ECS_ENABLED;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	return S_OK;
}
CATCH_RETURN();

IFACEMETHODIMP BaseExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags)
{
	*flags = ECF_DEFAULT;
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