#include "pch.h"
#include "CustomSubExplorerCommand.h"
#include "PathHelper.hpp"

using namespace winrt::Windows::Data::Json;

CustomSubExplorerCommand::CustomSubExplorerCommand(winrt::hstring const& configContent) {
	JsonObject result;
	if (JsonObject::TryParse(configContent, result)) {
		_title = result.GetNamedString(L"title", L"Custom Menu");
		_exe = result.GetNamedString(L"exe", L"");
		_param = result.GetNamedString(L"param", L"");
		_icon = result.GetNamedString(L"icon", L"");
		_accept_directory = result.GetNamedBoolean(L"acceptDirectory", false);
		_accept_exts = result.GetNamedString(L"acceptExts", L"");
		_accept_multiple_files = result.GetNamedBoolean(L"acceptMultipleFiles", false);
		_path_delimiter = result.GetNamedString(L"pathDelimiter", L"");
		_param_for_multiple_files = result.GetNamedString(L"paramForMultipleFiles", L"");
	}
}

bool CustomSubExplorerCommand::Accept(bool multipeFiles, bool isDirectory, const std::wstring& ext) {
	if (multipeFiles) {
		return _accept_multiple_files;
	}

	if (isDirectory) {
		return _accept_directory;
	}

	if (ext.empty()) {
		return true;
	}

	if (_accept_exts.empty()) {
		return true;
	}

	if (_accept_exts.find(L"*") != std::wstring::npos) {
		return true;
	}

	return _accept_exts.find(ext) != std::wstring::npos;
}

IFACEMETHODIMP CustomSubExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon)
{
	*icon = nullptr;
	if (!_icon.empty()) {
		return SHStrDupW(_icon.c_str(), icon);
	}
	else {
		return BaseExplorerCommand::GetIcon(items, icon);
	}
}


IFACEMETHODIMP CustomSubExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	if (_title.empty()) {
		return SHStrDupW(L"Menu", name);
	}
	else {
		return SHStrDupW(_title.c_str(), name);
	}
}

IFACEMETHODIMP CustomSubExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	*cmdState = ECS_ENABLED;
	return S_OK;
}


IFACEMETHODIMP CustomSubExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
{
	HWND parent = nullptr;
	if (m_site)
	{
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.Get(), &parent));
	}

	if (selection)
	{
		DWORD count;
		selection->GetCount(&count);

		if (count > 1 && _accept_multiple_files) {
			auto paths = PathHelper::getPaths(selection,_path_delimiter);
			if (!paths.empty()) {
				auto param = _param_for_multiple_files.empty()? std::wstring{ _param } : std::wstring{ _param_for_multiple_files };
				//get parent from first path
				if (param.find(L"{parent}") != std::wstring::npos) {
					auto path = PathHelper::getPath(selection);
					if (!path.empty()) {
						std::filesystem::path file(path);
						PathHelper::replaceAll(param, L"{parent}", file.parent_path().wstring());
					}
				}

				PathHelper::replaceAll(param, L"{path}", paths);
				//MessageBox(parent, param.c_str(), L"", 0);
				ShellExecute(parent, L"open", _exe.c_str(), param.c_str(), nullptr, SW_SHOWNORMAL);
			}
		}
		else if (count > 0) {
			auto path = PathHelper::getPath(selection);
			if (!path.empty()) {
				std::filesystem::path file(path);

				std::wstring param{ _param };
				PathHelper::replaceAll(param, L"{path}", file.wstring());

				if (param.find(L"{name}") != std::wstring::npos) {
					PathHelper::replaceAll(param, L"{name}", file.filename().wstring());
				}

				if (param.find(L"{parent}") != std::wstring::npos) {
					PathHelper::replaceAll(param, L"{parent}", file.parent_path().wstring());
				}
				//MessageBox(parent, param.c_str(), L"", 0);
				ShellExecute(parent, L"open", _exe.c_str(), param.c_str(), nullptr, SW_SHOWNORMAL);
			}
		}

	}
	return S_OK;
}
CATCH_RETURN();