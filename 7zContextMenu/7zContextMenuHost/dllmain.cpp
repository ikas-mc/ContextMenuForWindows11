// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <shobjidl_core.h>
#include <wil\resource.h>
#include <string>
#include <vector>
#include <sstream>
#include <shellapi.h>
#include <filesystem>
#include "SzExplorerCommand.h"
#include "VsCodeExplorerCommand.h"

#define _HAS_CXX17 1
#define _HAS_CXX20 0
using namespace Microsoft::WRL;

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

class TestExplorerCommandBase : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
    virtual const wchar_t* Title() = 0;
    virtual const EXPCMDFLAGS Flags() { return ECF_DEFAULT; }
    virtual const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }

    // IExplorerCommand
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;
        auto title = wil::make_cotaskmem_string_nothrow(Title());
        RETURN_IF_NULL_ALLOC(title);
        *name = title.release();
        return S_OK;
    }
    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) { *icon = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) { *infoTip = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) { *guidCommandName = GUID_NULL;  return S_OK; }
    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
        *cmdState = State(selection);
        return S_OK;
    }
    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
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
        MessageBox(parent, title.str().c_str(), L"ContextMenuCommand", MB_OK);
        return S_OK;
    }
    CATCH_RETURN();

    IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags) { *flags = Flags(); return S_OK; }
    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) { *enumCommands = nullptr; return E_NOTIMPL; }

    // IObjectWithSite
    IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept { m_site = site; return S_OK; }
    IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept { return m_site.CopyTo(riid, site); }

protected:
    ComPtr<IUnknown> m_site;
};

class __declspec(uuid("46F650E5-9959-48D6-AC13-A9637C5B3787")) TestExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"7-Zip Extract Here"; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_ENABLED; }
};

class __declspec(uuid("E6FC8DB3-36B6-451C-BFEA-2B5FF9345055")) TestExplorerCommand2Handler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"7z 1.2"; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_HIDDEN; }
};

class SubExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"7z 1.3.1"; }

};

class CheckedSubExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"7z 1.3.2"; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_CHECKBOX | ECS_CHECKED; }
};

class RadioCheckedSubExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"7z 1.3.3"; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_CHECKBOX | ECS_RADIOCHECK; }
};

class HiddenSubExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"HiddenSubCommand"; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_HIDDEN; }
};

class EnumCommands : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
    EnumCommands()
    {
        m_commands.push_back(Make<SubExplorerCommandHandler>());
        m_commands.push_back(Make<CheckedSubExplorerCommandHandler>());
        m_commands.push_back(Make<VsCodeExplorerCommand>());
        m_commands.push_back(Make<SzExplorerCommand>());
        m_current = m_commands.cbegin();
    }

    // IEnumExplorerCommand
    IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
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

    IFACEMETHODIMP Skip(ULONG /*celt*/) { return E_NOTIMPL; }
    IFACEMETHODIMP Reset()
    {
        m_current = m_commands.cbegin();
        return S_OK;
    }
    IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum) { *ppenum = nullptr; return E_NOTIMPL; }

private:
    std::vector<ComPtr<IExplorerCommand>> m_commands;
    std::vector<ComPtr<IExplorerCommand>>::const_iterator m_current;
};

class __declspec(uuid("B9775729-E5FD-43E2-B6B5-60EA19D6BBD5")) TestExplorerCommand3Handler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"More Menu"; }
    const EXPCMDFLAGS Flags() override { return ECF_HASSUBCOMMANDS; }
    const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override { return ECS_ENABLED; }

    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
    {
        *enumCommands = nullptr;
        auto e = Make<EnumCommands>();
        return e->QueryInterface(IID_PPV_ARGS(enumCommands));
    }
};


CoCreatableClass(TestExplorerCommandHandler)
CoCreatableClass(TestExplorerCommand2Handler)
CoCreatableClass(TestExplorerCommand3Handler)


CoCreatableClassWrlCreatorMapInclude(TestExplorerCommandHandler)
CoCreatableClassWrlCreatorMapInclude(TestExplorerCommand2Handler)
CoCreatableClassWrlCreatorMapInclude(TestExplorerCommand3Handler)


STDAPI DllGetActivationFactory(_In_ HSTRING activatableClassId, _COM_Outptr_ IActivationFactory** factory)
{
    return Module<ModuleType::InProc>::GetModule().GetActivationFactory(activatableClassId, factory);
}

STDAPI DllCanUnloadNow()
{
    return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _COM_Outptr_ void** instance)
{
    return Module<InProc>::GetModule().GetClassObject(rclsid, riid, instance);
}