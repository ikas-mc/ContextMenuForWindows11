#pragma once
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <filesystem>

#define DEBUG_LOG(message, ...) if(m_enable_debug) { \
	OutputDebugStringW(std::format(message, __VA_ARGS__).c_str());\
}

using namespace Microsoft::WRL;

enum ThemeType {
	Light = 0,
	Dark = 1
};

enum FileType {
	File = 0,
	Directory = 1,
	Background = 2,
	Desktop = 3,
	Drive = 4,
};

class BaseExplorerCommand : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite> {
public:
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) override;
	IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) override;
	IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) override;
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept override;
	IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags) override;
	IFACEMETHODIMP EnumSubCommands(__RPC__deref_out_opt IEnumExplorerCommand** enumCommands) override;
	IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept override;
	IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept override;

protected:
	wil::com_ptr_nothrow<IUnknown> m_site;
	ThemeType m_theme_type{ Light };
	bool m_enable_debug{ true };
};
