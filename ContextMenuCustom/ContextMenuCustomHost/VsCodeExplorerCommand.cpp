#include "pch.h"
#include "VsCodeExplorerCommand.h"

const  wchar_t* VsCodeExplorerCommand::Title() { return L"Open in vscode"; };
const EXPCMDSTATE VsCodeExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
IFACEMETHODIMP VsCodeExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.Get(), &parent));
	}

	wil::unique_cotaskmem_string path = GetPath(selection);

	if (path.is_valid()) {
		auto param{ wil::str_printf<std::wstring>(LR"-("%s"")-", path.get()) };
		ShellExecute(NULL, L"open", L"code", param.c_str(), NULL, SW_HIDE);
	}

	return S_OK;
}
CATCH_RETURN();