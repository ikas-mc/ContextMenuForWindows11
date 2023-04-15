#include "pch.h"
#include "CustomSubExplorerCommand.h"
#include "PathHelper.hpp"
#include <regex>

using namespace winrt::Windows::Data::Json;

CustomSubExplorerCommand::CustomSubExplorerCommand(winrt::hstring const& configContent) : _accept_directory(false), _accept_file(false), _accept_multiple_files_flag(0), m_index(0)
{
	JsonObject result;
	if (JsonObject::TryParse(configContent, result))
	{
		_title = result.GetNamedString(L"title", L"Custom Menu");
		_exe = result.GetNamedString(L"exe", L"");
		_param = result.GetNamedString(L"param", L"");
		_icon = result.GetNamedString(L"icon", L"");
        m_index = (int)result.GetNamedNumber(L"index", 0);

		_accept_directory = result.GetNamedBoolean(L"acceptDirectory", false);

		_accept_file = result.GetNamedBoolean(L"acceptFile", false); //v3.6, next to remove
        _accept_file_flag = (int)result.GetNamedNumber(L"acceptFileFlag", 0);
        _accept_file_regex = result.GetNamedString(L"acceptFileRegex", L"");
		_accept_exts = result.GetNamedString(L"acceptExts", L"");
        //
        if (_accept_file_flag == 0 && _accept_file) {
            _accept_file_flag = MATCH_FILE_EXT;
        }

        _accept_multiple_files_flag = (int)result.GetNamedNumber(L"acceptMultipleFilesFlag", 0);
        _path_delimiter = result.GetNamedString(L"pathDelimiter", L"");
		_param_for_multiple_files = result.GetNamedString(L"paramForMultipleFiles", L"");
	}
}

bool CustomSubExplorerCommand::Accept(bool multipleFiles, bool isDirectory, const std::wstring& name, const std::wstring& ext)
{
	if (multipleFiles)
	{
		return _accept_multiple_files_flag == MultipleFilesFlagJOIN || _accept_multiple_files_flag == MultipleFilesFlagEACH;
	}

	if (isDirectory)
	{
		return _accept_directory;
	}

	if (_accept_file_flag == MATCH_FILE_EXT)
	{
		if (ext.empty() || _accept_exts.empty())
		{
			return true;
		}

		if (_accept_exts.find(L'*') != std::wstring::npos)
		{
			return true;
		}

        //TODO split first, .c .cpp bug
		return _accept_exts.find(ext) != std::wstring::npos;
	}
	else if (_accept_file_flag == MATCH_FILE_REGEX)
	{
		if (_accept_file_regex.empty())
		{
			return true;
		}

		std::wregex fileRegex(_accept_file_regex);
		return  std::regex_match(name, fileRegex);
	}

	return false;
}

IFACEMETHODIMP CustomSubExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon)
{
	*icon = nullptr;
	if (!_icon.empty())
	{
		return SHStrDupW(_icon.c_str(), icon);
	}
	else
	{
		return BaseExplorerCommand::GetIcon(items, icon);
	}
}

IFACEMETHODIMP CustomSubExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
{
	*name = nullptr;
	if (_title.empty())
	{
		return SHStrDupW(L"Menu", name);
	}
	else
	{
		return SHStrDupW(_title.c_str(), name);
	}
}

IFACEMETHODIMP CustomSubExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
{
	*cmdState = ECS_ENABLED;
	return S_OK;
}

IFACEMETHODIMP CustomSubExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept
try
{
	HWND parent = nullptr;
	if (m_site)
	{
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.get(), &parent));
	}

	if (selection)
	{
		DWORD count;
		selection->GetCount(&count);

		if (count > 1 && _accept_multiple_files_flag == MultipleFilesFlagJOIN)
		{
			auto paths = PathHelper::getPaths(selection, _path_delimiter);
			if (!paths.empty())
			{
				auto param = _param_for_multiple_files.empty() ? std::wstring{ _param } : std::wstring{ _param_for_multiple_files };
				// get parent from first path

				std::wstring parentPath;
				auto firstPath = PathHelper::getPath(selection);
				if (!firstPath.empty())
				{
					std::filesystem::path file(firstPath);
					parentPath = file.parent_path().wstring();
				}

				if (param.find(PARAM_PARENT) != std::wstring::npos)
				{
					PathHelper::replaceAll(param, PARAM_PARENT, parentPath);
				}

				PathHelper::replaceAll(param, PARAM_PATH, paths);
				auto exePath = wil::ExpandEnvironmentStringsW(_exe.c_str());
				ShellExecute(parent, L"open", exePath.get(), param.c_str(), parentPath.data(), SW_SHOWNORMAL);
			}
		}
		else if (count > 1 && _accept_multiple_files_flag == MultipleFilesFlagEACH)
		{
			auto paths = PathHelper::getPathList(selection);
			if (!paths.empty())
			{
				for (auto& path : paths)
				{
					if (path.empty())
					{
						continue;
					}
					Execute(parent, path);
				}
			}
		}
		else if (count > 0)
		{
			auto path = PathHelper::getPath(selection);
			Execute(parent, path);
		}
	}
	return S_OK;
}
CATCH_RETURN();

void CustomSubExplorerCommand::Execute(HWND parent, const std::wstring& path)
{
	if (!path.empty())
	{
		auto param = std::wstring{ _param };

		std::filesystem::path file(path);

		if (param.find(PARAM_PARENT) != std::wstring::npos)
		{
			PathHelper::replaceAll(param, PARAM_PARENT, file.parent_path().wstring());
		}
		if (param.find(PARAM_NAME) != std::wstring::npos)
		{
			PathHelper::replaceAll(param, PARAM_NAME, file.filename().wstring());
		}

		PathHelper::replaceAll(param, PARAM_PATH, path);

		auto exePath = wil::ExpandEnvironmentStringsW(_exe.c_str());
		ShellExecute(parent, L"open", exePath.get(), param.c_str(), file.parent_path().c_str(), SW_SHOWNORMAL);
	}
}