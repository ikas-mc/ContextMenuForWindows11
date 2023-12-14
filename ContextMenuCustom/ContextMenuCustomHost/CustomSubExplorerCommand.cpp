#include "pch.h"
#include "CustomSubExplorerCommand.h"
#include "PathHelper.hpp"
#include <regex>
#include <shellapi.h>
#include <winrt/Windows.Data.Json.h>

using namespace winrt::Windows::Data::Json;

CustomSubExplorerCommand::CustomSubExplorerCommand(const winrt::hstring& configContent) :
	_accept_directory(false)
	, _accept_file(false)
	, _accept_multiple_files_flag(0)
	, m_index(0) {
	if (JsonObject result; JsonObject::TryParse(configContent, result)) {
		_title = result.GetNamedString(L"title", L"Custom Menu");
		_exe = result.GetNamedString(L"exe", L"");
		_param = result.GetNamedString(L"param", L"");
		_icon = result.GetNamedString(L"icon", L"");
		m_index = static_cast<int>(result.GetNamedNumber(L"index", 0));

		_accept_directory = result.GetNamedBoolean(L"acceptDirectory", false);

		_accept_file = result.GetNamedBoolean(L"acceptFile", false); //v3.6, next to remove
		_accept_file_flag = static_cast<int>(result.GetNamedNumber(L"acceptFileFlag", 0));
		_accept_file_regex = result.GetNamedString(L"acceptFileRegex", L"");
		_accept_exts = result.GetNamedString(L"acceptExts", L"");
		//
		if (_accept_file_flag == 0 && _accept_file) {
			_accept_file_flag = MATCH_FILE_EXT;
		}

		_accept_multiple_files_flag = static_cast<int>(result.GetNamedNumber(L"acceptMultipleFilesFlag", 0));
		_path_delimiter = result.GetNamedString(L"pathDelimiter", L"");
		_param_for_multiple_files = result.GetNamedString(L"paramForMultipleFiles", L"");
	}
}

bool CustomSubExplorerCommand::Accept(const bool multipleFiles, const bool isDirectory, const std::wstring& name, const std::wstring& ext) {
	if (multipleFiles) {
		return _accept_multiple_files_flag == MULTIPLE_FILES_FLAG_JOIN || _accept_multiple_files_flag == MULTIPLE_FILES_FLAG_EACH;
	}

	if (isDirectory) {
		return _accept_directory;
	}

	if (_accept_file_flag == MATCH_FILE_EXT) {
		if (ext.empty() || _accept_exts.empty()) {
			return true;
		}

		if (_accept_exts.find(L'*') != std::wstring::npos) {
			return true;
		}

		//TODO split first, .c .cpp bug
		return _accept_exts.find(ext) != std::wstring::npos;
	}
	if (_accept_file_flag == MATCH_FILE_REGEX) {
		if (_accept_file_regex.empty()) {
			return true;
		}

		const std::wregex fileRegex(_accept_file_regex);
		return std::regex_match(name, fileRegex);
	}

	return false;
}

IFACEMETHODIMP CustomSubExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon) {
	*icon = nullptr;
	if (!_icon.empty()) {
		return SHStrDupW(_icon.c_str(), icon);
	}
	return BaseExplorerCommand::GetIcon(items, icon);
}

IFACEMETHODIMP CustomSubExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) {
	*name = nullptr;
	if (_title.empty()) {
		return SHStrDupW(L"Menu", name);
	}
	return SHStrDupW(_title.c_str(), name);
}

IFACEMETHODIMP CustomSubExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	*cmdState = ECS_ENABLED;
	return S_OK;
}

IFACEMETHODIMP CustomSubExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try {
	HWND parent = nullptr;
	if (m_site) {
		RETURN_IF_FAILED(IUnknown_GetWindow(m_site.get(), &parent));
	}

	if (selection) {
		DWORD count;
		selection->GetCount(&count);

		if (count > 1 && _accept_multiple_files_flag == MULTIPLE_FILES_FLAG_JOIN) {
			if (const auto paths = PathHelper::getPaths(selection, _path_delimiter); !paths.empty()) {
				auto param = _param_for_multiple_files.empty() ? std::wstring{_param} : std::wstring{_param_for_multiple_files};
				// get parent from first path

				std::wstring parentPath;
				if (const auto firstPath = PathHelper::getPath(selection); !firstPath.empty()) {
					const std::filesystem::path file(firstPath);
					parentPath = file.parent_path().wstring();
				}

				if (param.find(PARAM_PARENT) != std::wstring::npos) {
					PathHelper::replaceAll(param, PARAM_PARENT, parentPath);
				}

				PathHelper::replaceAll(param, PARAM_PATH, paths);
				const auto exePath = wil::ExpandEnvironmentStringsW(_exe.c_str());
				ShellExecute(parent, L"open", exePath.get(), param.c_str(), parentPath.data(), SW_SHOWNORMAL);
			}
		} else if (count > 1 && _accept_multiple_files_flag == MULTIPLE_FILES_FLAG_EACH) {
			if (const auto paths = PathHelper::getPathList(selection); !paths.empty()) {
				for (auto& path : paths) {
					if (path.empty()) {
						continue;
					}
					Execute(parent, path);
				}
			}
		} else if (count > 0) {
			const auto path = PathHelper::getPath(selection);
			Execute(parent, path);
		}
	}
	return S_OK;
}CATCH_RETURN();

void CustomSubExplorerCommand::Execute(HWND parent, const std::wstring& path) {
	if (!path.empty()) {
		auto param = std::wstring{_param};

		const std::filesystem::path file(path);
		/*
		//TODO parser
		PathHelper::replaceAll(param, PARAM_PARENT, file.parent_path().wstring());
		PathHelper::replaceAll(param, PARAM_NAME, file.filename().wstring());
		PathHelper::replaceAll (param, PARAM_STEM, file.stem().wstring ());
		PathHelper::replaceAll(param, PARAM_PATH, path);
		*/
		/**/
		if (param.length () > 5) {
			std::wstring_view paramView{ param };
			std::wstringstream out;
			size_t pos = 0;
			for (;;) {
				const size_t substPos = paramView.find (L'{', pos);
				const size_t endPos = paramView.find (L'}', substPos);

				if (endPos == std::wstring::npos) {
					break;
				}

				out << paramView.substr (pos, substPos - pos);

				pos = endPos + 1;

				const auto key = paramView.substr (substPos, pos - substPos);
				if (key == PARAM_NAME) {
					out << file.filename ().wstring ();
				} else if (key == PARAM_PATH) {
					out << path;
				} else if (key == PARAM_PARENT) {
					out << file.parent_path ().wstring ();
				} else if (key == PARAM_NAME_NO_EXT) {
					out << file.stem ().wstring ();
				} else {
					//out << "";
				}
			}

			if (pos > 0) {
				out << paramView.substr (pos, std::wstring::npos);
				param = out.str ();
			}

		}
		
		const auto exePath = wil::ExpandEnvironmentStringsW(_exe.c_str());
		ShellExecute(parent, L"open", exePath.get(), param.c_str(), file.parent_path().c_str(), SW_SHOWNORMAL);
	}
}
