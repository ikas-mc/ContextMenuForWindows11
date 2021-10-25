#include "pch.h"
#include "SzExplorerCommand.h"


const  wchar_t* SzExplorerCommand::Title() { return L"7-Zip Extract Here"; };
const EXPCMDSTATE SzExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
IFACEMETHODIMP SzExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.Get(), &parent));
	}

	wil::unique_cotaskmem_string path = GetPath(selection);

	if (path.is_valid()) {
		std::wstring file = path.get();
		std::filesystem::path outFile(file);
		if (outFile.has_extension()) {
			outFile.replace_extension();
		}
		else {
			outFile += "~";
		}
		
		//auto output{ wil::str_printf<std::wstring>(LR"-("%s"")-", outFile.c_str()) };
		auto param{ wil::str_printf<std::wstring>(LR"-(x "%s" -o"%s"")-",file.c_str(),outFile.c_str()) };

		SHELLEXECUTEINFO ShExecInfo = { 0 };
		ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
		ShExecInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
		ShExecInfo.hwnd = parent;
		ShExecInfo.lpVerb = nullptr;
		ShExecInfo.lpFile = L"7zG.exe";
		ShExecInfo.lpParameters = param.c_str();
		ShExecInfo.lpDirectory = nullptr;
		ShExecInfo.nShow = SW_SHOW;
		ShExecInfo.hInstApp = nullptr;
		ShellExecuteEx(&ShExecInfo);
		//if (ShExecInfo.hProcess > 0) {
			//WaitForSingleObject(ShExecInfo.hProcess, INFINITE);
			//open output folder
			//ShellExecute(parent, L"open", output.c_str(), nullptr, nullptr, SW_SHOWNORMAL);
		//}
	}

	return S_OK;
}

CATCH_RETURN();