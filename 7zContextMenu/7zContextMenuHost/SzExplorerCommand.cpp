#include "pch.h"
#include "SzExplorerCommand.h"


const  wchar_t* SzExplorerCommand::Title()  { return L"7-Zip Extract Here"; };
const EXPCMDSTATE SzExplorerCommand::State(_In_opt_ IShellItemArray * selection)  { return ECS_ENABLED; };
IFACEMETHODIMP SzExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
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
                //get full path
                hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
                if (SUCCEEDED(hr))
                {
                    std::filesystem::path p(path);
                    auto file = p.wstring();
                    if (p.has_extension()) {
                        p.replace_extension();
                    }
                    else {
                        p += "~";
                    }

                    auto output = L"\"" + p.wstring() + L"\"";
                    auto param = L"x \"" + file + L"\" -o" + output;
                    //7zG
                    SHELLEXECUTEINFO ShExecInfo = { 0 };
                    ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
                    ShExecInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
                    ShExecInfo.hwnd = NULL;
                    ShExecInfo.lpVerb = NULL;
                    ShExecInfo.lpFile = L"7zG.exe";
                    ShExecInfo.lpParameters = param.c_str();
                    ShExecInfo.lpDirectory = NULL;
                    ShExecInfo.nShow = SW_SHOW;
                    ShExecInfo.hInstApp = NULL;
                    ShellExecuteEx(&ShExecInfo);
                    if (ShExecInfo.hProcess > 0) {
                        WaitForSingleObject(ShExecInfo.hProcess, INFINITE);
                        //open output folder
                        ShellExecute(NULL, L"open", output.c_str(), NULL, NULL, SW_SHOWNORMAL);
                    }
                }
                item->Release();
            }
        }
        // title << L" (" << count << L" selected items)";
    }
    else
    {
        title << L"(no selected items)";
        MessageBox(parent, title.str().c_str(), L"TestCommand", MB_OK);
    }

    return S_OK;
}
CATCH_RETURN();