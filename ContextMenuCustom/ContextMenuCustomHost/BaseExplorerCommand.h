#pragma once
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <shobjidl_core.h>
#include <string>
#include <vector>
#include <sstream>
#include <shellapi.h>
#include <filesystem>
#include <wil/Common.h>
#include <wil/Result.h>
#include <wil/nt_result_macros.h>
#include <wil/resource.h>
#include <wil/wistd_memory.h>
#include <wil/stl.h>
#include <wil/filesystem.h>
#include <wil/win32_helpers.h>
#include <shlwapi.h>

using namespace Microsoft::WRL;
class BaseExplorerCommand : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
	const virtual wchar_t* Title() = 0;
	const virtual EXPCMDFLAGS Flags();
	const virtual EXPCMDSTATE State(_In_opt_ IShellItemArray* selection);
	const virtual wchar_t* GetIconId();

	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name);

	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon);
	IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip);
	IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName);
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState);

	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept;

	IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags);
	IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands);


	IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept;
	IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept;

	static wil::unique_cotaskmem_string GetPath(IShellItemArray* selection) {
		wil::unique_cotaskmem_string path;
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0) {
				IShellItem* item;
				if (SUCCEEDED(selection->GetItemAt(0, &item))) {
					item->GetDisplayName(SIGDN_FILESYSPATH, path.put());
					item->Release();
				}
			}
		}
		return path;
	}
protected:
	ComPtr<IUnknown> m_site;
	
};