#include "pch.h"
#include "BaseExplorerCommand.h"

IFACEMETHODIMP BaseExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) {
	*name = nullptr;
	auto title = wil::make_cotaskmem_string_nothrow(L"Open With");
	RETURN_IF_NULL_ALLOC(title);
	*name = title.release();
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon) {
	*icon = nullptr;
	const auto customIcon = winrt::unbox_value_or<winrt::hstring>(winrt::Windows::Storage::ApplicationData::Current().LocalSettings().Values().Lookup(m_theme_type == ThemeType::Dark ? L"Custom_Menu_Dark_Icon" : L"Custom_Menu_Light_Icon"), L"");
	if (customIcon.empty()) {
		const std::filesystem::path modulePath{ wil::GetModuleFileNameW<std::wstring>(wil::GetModuleInstanceHandle()) };
		auto iconPath = wil::make_cotaskmem_string_nothrow((modulePath.wstring() + (m_theme_type == ThemeType::Dark ? L",-101" : L",-103")).c_str());
		RETURN_IF_NULL_ALLOC(iconPath);
		*icon = iconPath.release();
	}
	else {
		DEBUG_LOG(L"BaseExplorerCommand::GetIcon ,m_theme_type={},custom icon={}", static_cast<int>(m_theme_type), customIcon);
		auto iconPath = wil::make_cotaskmem_string_nothrow(customIcon.c_str());
		RETURN_IF_NULL_ALLOC(iconPath);
		*icon = iconPath.release();
	}
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) {
	*infoTip = nullptr;
	return E_NOTIMPL;
}

IFACEMETHODIMP BaseExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName) {
	*guidCommandName = GUID_NULL;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	*cmdState = ECS_ENABLED;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try {
	return S_OK;
}CATCH_RETURN();

IFACEMETHODIMP BaseExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags) {
	*flags = ECF_DEFAULT;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) {
	*enumCommands = nullptr;
	return E_NOTIMPL;
}

IFACEMETHODIMP BaseExplorerCommand::SetSite(_In_ IUnknown* site) noexcept {
	m_site = site;
	return S_OK;
}

IFACEMETHODIMP BaseExplorerCommand::GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept {
	RETURN_IF_FAILED(m_site.query_to(riid, site));
	return S_OK;
}
