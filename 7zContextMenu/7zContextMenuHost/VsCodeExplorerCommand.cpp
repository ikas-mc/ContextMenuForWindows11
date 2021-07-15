#include "pch.h"
#include "VsCodeExplorerCommand.h"

const  wchar_t* VsCodeExplorerCommand::Title() { return L"Open in vscode"; };
const EXPCMDSTATE VsCodeExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
IFACEMETHODIMP VsCodeExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		ComPtr<IOleWindow> oleWindow;
		RETURN_IF_FAILED(m_site.As(&oleWindow));
		RETURN_IF_FAILED(oleWindow->GetWindow(&parent));
	}

	if (selection)
	{
		DWORD count;
		RETURN_IF_FAILED(selection->GetCount(&count));
		if (count > 0) {
			IShellItem* item;
			auto hr = selection->GetItemAt(0, &item);
			if (SUCCEEDED(hr)) {
				LPOLESTR path = NULL;
				//get full path
				hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
				if (SUCCEEDED(hr))
				{
					std::wostringstream param;
					param << L"\"" << path << L"\"";
					ShellExecute(NULL, L"open", L"code", param.str().c_str(), NULL, SW_HIDE);
				}
				item->Release();
			}
		}
	}
	else
	{
		MessageBox(parent, L"(no selected items)", L"vscode menu", MB_OK);
	}

	return S_OK;
}
CATCH_RETURN();