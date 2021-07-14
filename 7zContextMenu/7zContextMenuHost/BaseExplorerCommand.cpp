#include "pch.h"
#include "BaseExplorerCommand.h"

const  wchar_t* BaseExplorerCommand::Title() { return L""; }
const EXPCMDFLAGS BaseExplorerCommand::Flags() { return ECF_DEFAULT; }
const EXPCMDSTATE BaseExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }

// IExplorerCommand
IFACEMETHODIMP BaseExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
    *name = nullptr;
    auto title = wil::make_cotaskmem_string_nothrow(Title());
    RETURN_IF_NULL_ALLOC(title);
    *name = title.release();
    return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) { *icon = nullptr; return E_NOTIMPL; }
IFACEMETHODIMP BaseExplorerCommand::GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) { *infoTip = nullptr; return E_NOTIMPL; }
IFACEMETHODIMP BaseExplorerCommand::GetCanonicalName(_Out_ GUID* guidCommandName) { *guidCommandName = GUID_NULL;  return S_OK; }
IFACEMETHODIMP BaseExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
{
    *cmdState = State(selection);
    return S_OK;
}
IFACEMETHODIMP BaseExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept { return S_OK; };


IFACEMETHODIMP BaseExplorerCommand::GetFlags(_Out_ EXPCMDFLAGS* flags) { *flags = Flags(); return S_OK; }
IFACEMETHODIMP BaseExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) { *enumCommands = nullptr; return E_NOTIMPL; }

// IObjectWithSite
    // IObjectWithSite
IFACEMETHODIMP BaseExplorerCommand::SetSite(_In_ IUnknown* site) noexcept { m_site = site; return S_OK; }
IFACEMETHODIMP BaseExplorerCommand::GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept { return m_site.CopyTo(riid, site); }