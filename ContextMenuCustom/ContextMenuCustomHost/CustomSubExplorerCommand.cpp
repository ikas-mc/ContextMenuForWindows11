#include "pch.h"
#include "CustomSubExplorerCommand.h"
#include "PathHelper.hpp"
#include <regex>

using namespace winrt::Windows::Data::Json;

CustomSubExplorerCommand::CustomSubExplorerCommand(const winrt::hstring& configContent, ThemeType themeType, bool enableDebug)
{
	m_theme_type = themeType;
	m_enable_debug = enableDebug;
	try
	{
		const JsonObject result = JsonObject::Parse(configContent);
		_title = result.GetNamedString(L"title", L"Custom Menu");
		_exe = result.GetNamedString(L"exe", L"");
		_param = result.GetNamedString(L"param", L"");
		_icon = result.GetNamedString(L"icon", L"");
		_icon_dark = result.GetNamedString(L"iconDark", L"");
		m_index = static_cast<int>(result.GetNamedNumber(L"index", 0));

		_accept_directory = result.GetNamedBoolean(L"acceptDirectory", false);
		_accept_directory_flag = static_cast<int>(result.GetNamedNumber(L"acceptDirectoryFlag", 0));

		_accept_file = result.GetNamedBoolean(L"acceptFile", false); //v3.6, next to remove
		_accept_file_flag = static_cast<int>(result.GetNamedNumber(L"acceptFileFlag", 0));
		_accept_file_regex = result.GetNamedString(L"acceptFileRegex", L"");
		_accept_exts = result.GetNamedString(L"acceptExts", L"");

		_accept_multiple_files_flag = static_cast<int>(result.GetNamedNumber(L"acceptMultipleFilesFlag", 0));
		_path_delimiter = result.GetNamedString(L"pathDelimiter", L"");
		_param_for_multiple_files = result.GetNamedString(L"paramForMultipleFiles", L"");

		//
		_show_window_flag = static_cast<int>(result.GetNamedNumber(L"showWindowFlag", 0));
		_run_as_flag = static_cast<int>(result.GetNamedNumber(L"runAsFlag", 0));
		_working_directory = result.GetNamedString(L"workingDirectory", L"");

		//TODO remove next version
		if (_accept_file_flag == 0 && _accept_file) {
			_accept_file_flag = FILE_EXT;
		}
		//TODO remove next version
		if (_accept_directory_flag == 0 && _accept_directory) {
			_accept_directory_flag = DIRECTORY_DIRECTORY | DIRECTORY_BACKGROUND | DIRECTORY_DESKTOP;
		}

		if (_accept_file_flag == FileMatchFlagEnum::FILE_EXT2 && !_accept_exts.empty()) {
			std::wstring_view acceptExtsView{ _accept_exts };

			for (std::size_t start = 0; start <= acceptExtsView.size();)
			{
				const auto pos = acceptExtsView.find(L'|', start);
				const auto last = pos == std::wstring_view::npos;
				std::wstring_view token = last ? acceptExtsView.substr(start) : acceptExtsView.substr(start, pos - start);
				if (!token.empty()) {
					_accept_exts_set.emplace(token);
				}
				if (last) {
					break;
				}
				start = pos + 1;
			}
		}
	}
	catch (winrt::hresult_error const& e)
	{
		_title = _title + L" (config parse error)";
		DEBUG_LOG(L"CustomSubExplorerCommand::CustomSubExplorerCommand parse error,message={}, json={}", e.message(), configContent);
	}
}

bool CustomSubExplorerCommand::Accept(bool multipleFiles, FileType fileType, const std::wstring& name, const std::wstring& ext) {
	DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, isMultipleFiles={}, fileType={}, fileName={}, fileExt={}", _title, multipleFiles, static_cast<int>(fileType), name, ext);

	if (multipleFiles) {
		DEBUG_LOG(L"CustomSubExplorerCommand::Accept multiple_files_flag={}", _accept_multiple_files_flag);
		return _accept_multiple_files_flag == FILES_JOIN || _accept_multiple_files_flag == FILES_EACH;
	}

	//file
	if (fileType == FileType::File) {
		if (_accept_file_flag == FileMatchFlagEnum::FILE_ALL) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={},file=all", _title);
			return true;
		}
		else if (_accept_file_flag == FileMatchFlagEnum::FILE_EXT) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, file=ext like, ext={}", _title, _accept_exts);
			if (ext.empty() || _accept_exts.empty()) {
				return true;
			}
			if (_accept_exts.find(L'*') != std::wstring::npos) {
				return true;
			}
			return _accept_exts.find(ext) != std::wstring::npos;
		}
		else if (_accept_file_flag == FileMatchFlagEnum::FILE_EXT2) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, file=ext list, ext={}", _title, _accept_exts);
			if (ext.empty() || _accept_exts.empty()) {
				return false;
			}

			return _accept_exts_set.contains(ext);
		}
		else if (_accept_file_flag == FileMatchFlagEnum::FILE_REGEX) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, file=regex, ext={}", _title, _accept_file_regex);
			if (_accept_file_regex.empty()) {
				return false;
			}

			const std::wregex fileRegex(_accept_file_regex);
			return std::regex_match(name, fileRegex);
		}
	}
	//directory
	else if (fileType == FileType::Directory) {
		DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, directory=Directory", _title);
		return (_accept_directory_flag & DIRECTORY_DIRECTORY) == DIRECTORY_DIRECTORY;
	}
	//background
	else if (fileType == FileType::Background) {
		DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, directory=Background", _title);
		return  (_accept_directory_flag & DIRECTORY_BACKGROUND) == DIRECTORY_BACKGROUND;
	}
	//desktop
	else if (fileType == FileType::Desktop) {
		DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, directory=Desktop", _title);
		return  (_accept_directory_flag & DIRECTORY_DESKTOP) == DIRECTORY_DESKTOP;
	}
	//drive
	else if (fileType == FileType::Drive) {
		DEBUG_LOG(L"CustomSubExplorerCommand::Accept menu={}, directory=Drive", _title);
		return  (_accept_directory_flag & DIRECTORY_DRIVE) == DIRECTORY_DRIVE;
	}

	DEBUG_LOG(L"CustomSubExplorerCommand::Accept skip, menu={}", _title);
	return false;
}

bool CustomSubExplorerCommand::AcceptPath(const std::wstring& path) {
	if (path.empty()) {
		return false;
	}

	bool isDirectory = false;
	std::wstring name;
	std::wstring ext;
	PathHelper::getExt(path, isDirectory, name, ext);
	if (name.empty()) {
		return false;
	}

	if (!isDirectory) {
		return Accept(false, FileType::File, name, ext);
	}

	FileType fileType = FileType::Directory;
	const auto pathLength = path.length();
	if (pathLength == 2 || pathLength == 3 && PathIsRoot(path.data())) {
		fileType = FileType::Drive;
	}
	return Accept(false, fileType, name, ext);
}

std::vector<std::wstring> CustomSubExplorerCommand::FilterAcceptedPaths(IShellItemArray* selection) {
	std::vector<std::wstring> acceptedPaths;
	if (const auto paths = PathHelper::getPathList(selection); !paths.empty()) {
		for (const auto& path : paths) {
			if (AcceptPath(path)) {
				acceptedPaths.emplace_back(path);
			}
		}
	}
	return acceptedPaths;
}

bool CustomSubExplorerCommand::AcceptAny(IShellItemArray* selection) {
	if (!Accept(true, FileType::File, L"", L"")) {
		return false;
	}

	const auto acceptedPaths = FilterAcceptedPaths(selection);
	return !acceptedPaths.empty();
}

IFACEMETHODIMP CustomSubExplorerCommand::GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* icon) {
	wil::assign_null_to_opt_param(icon);

	std::wstring_view iconPath{ m_theme_type == ThemeType::Dark && !_icon_dark.empty() ? _icon_dark : _icon };
	if (iconPath.find(L"%") != std::string::npos)
	{
		wil::unique_cotaskmem_string path{};
		if (S_OK == wil::ExpandEnvironmentStringsW(iconPath.data(), path))
		{
			*icon = path.release();
			return S_OK;
		}
	}
	else if (!iconPath.empty())
	{
		auto path{ wil::make_cotaskmem_string(iconPath.data(), iconPath.size()) };
		*icon = path.release();
		return S_OK;
	}

	return BaseExplorerCommand::GetIcon(items, icon);
}

IFACEMETHODIMP CustomSubExplorerCommand::GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) {
	wil::assign_null_to_opt_param(name);
	auto title = wil::make_cotaskmem_string_nothrow(_title.c_str());
	RETURN_IF_NULL_ALLOC(title);
	*name = title.release();
	return S_OK;
}

IFACEMETHODIMP CustomSubExplorerCommand::GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) {
	*cmdState = ECS_ENABLED;
	return S_OK;
}

IFACEMETHODIMP CustomSubExplorerCommand::Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try {
	DEBUG_LOG(L"CustomSubExplorerCommand::Invoke , menu={}", _title);

	HWND parent = nullptr;
	if (m_site) {
		const auto ret = IUnknown_GetWindow(m_site.get(), &parent);
		if (FAILED(ret)) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Invoke, menu={}, GetWindow failed={}", _title, ret);
		}
	}

	DWORD count = 0;
	if (selection) {
		selection->GetCount(&count);
	}
	DEBUG_LOG(L"CustomSubExplorerCommand::Invoke  menu={}, selection size={}", _title, count);

	if (count > 1 && _accept_multiple_files_flag == FILES_JOIN) {
		if (const auto paths = FilterAcceptedPaths(selection); !paths.empty()) {
			std::wstring joinedPaths;
			joinedPaths.reserve(paths.size() * 8);
			for (size_t i = 0; i < paths.size(); ++i) {
				joinedPaths += L'"';
				joinedPaths += paths[i];
				joinedPaths += L'"';
				if (i + 1 < paths.size()) {
					joinedPaths += _path_delimiter;
				}
			}
			DEBUG_LOG(L"CustomSubExplorerCommand::Invoke menu={}, join, paths={}", _title, joinedPaths);

			const std::wstring_view paramView{ _param_for_multiple_files.empty() ? _param : _param_for_multiple_files };
			std::unordered_map<std::wstring_view, std::wstring> replacements(5);
			std::wstring parentPath;
			if (const auto& firstPath = paths.front(); !firstPath.empty()) {
				const std::filesystem::path file(firstPath);
				parentPath = file.parent_path().wstring();
				replacements.emplace(PARAM_PARENT, parentPath);
				replacements.emplace(PARAM_PATH, joinedPaths);
				//TODO
				replacements.emplace(PARAM_PATH0, firstPath);
				replacements.emplace(PARAM_NAME0, file.filename().wstring());
				replacements.emplace(PARAM_EXT0, file.extension().wstring());
				replacements.emplace(PARAM_NAME_NO_EXT0, file.stem().wstring());
			}
			std::wstring param = PathHelper::simpleFormat(paramView, replacements);

			std::wstring workingDirectory{ _working_directory.find(L"%") == std::string::npos ? _working_directory : wil::ExpandEnvironmentStringsW(_working_directory.c_str()).get() };
			if (workingDirectory.empty()) {
				workingDirectory = parentPath;
			}
			else {
				PathHelper::replaceAll(workingDirectory, PARAM_PARENT, replacements[PARAM_PARENT]);
				PathHelper::replaceAll(workingDirectory, PARAM_PATH0, replacements[PARAM_PATH0]);
			}
			DEBUG_LOG(L"CustomSubExplorerCommand::Invoke menu={}, workingDirectoryPath={}", _title, workingDirectory);

			const std::wstring exePath{ _exe.find(L"%") == std::string::npos ? _exe : wil::ExpandEnvironmentStringsW(_exe.c_str()).get() };
			DEBUG_LOG(L"CustomSubExplorerCommand::Invoke menu={}, exePath={}, param={}", _title, exePath, param);

			Execute(parent, exePath, param, workingDirectory);
		}
	}
	else if (count > 1 && _accept_multiple_files_flag == FILES_EACH) {
		if (const auto paths = FilterAcceptedPaths(selection); !paths.empty()) {
			DEBUG_LOG(L"CustomSubExplorerCommand::Invoke menu={}, each", _title);

			for (auto& path : paths) {
				if (path.empty()) {
					continue;
				}
				DoInvoke(parent, path);
			}
		}
	}
	else if (count > 0) {
		const auto path = PathHelper::getPath(selection);
		DoInvoke(parent, path);
	}

	return S_OK;
}CATCH_RETURN();

void CustomSubExplorerCommand::DoInvoke(HWND parent, const std::wstring& path) {
	DEBUG_LOG(L"CustomSubExplorerCommand::Execute menu={}, path={}", _title, path);

	if (path.empty()) {
		return;
	}

	const std::filesystem::path file(path);
	std::unordered_map<std::wstring_view, std::wstring> replacements = {
		{PARAM_PATH, path},
		{PARAM_PARENT, file.parent_path().wstring()},
		{PARAM_NAME, file.filename().wstring()},
		{PARAM_EXT, file.extension().wstring()},
		{PARAM_NAME_NO_EXT, file.stem().wstring()},
	};
	std::wstring param = PathHelper::simpleFormat(_param, replacements);

	// TODO
	std::wstring workingDirectory{ _working_directory.find(L"%") == std::string::npos ? _working_directory : wil::ExpandEnvironmentStringsW(_working_directory.c_str()).get() };
	if (workingDirectory.empty()) {
		workingDirectory = file.parent_path().wstring();
	}
	else {
		PathHelper::replaceAll(workingDirectory, PARAM_PARENT, replacements[PARAM_PARENT]);
		// TODO
		PathHelper::replaceAll(workingDirectory, PARAM_PATH, replacements[PARAM_PATH]);
		PathHelper::replaceAll(workingDirectory, PARAM_PATH0, replacements[PARAM_PATH]);
	}

	const std::wstring exePath{ _exe.find(L"%") == std::string::npos ? _exe : wil::ExpandEnvironmentStringsW(_exe.c_str()).get() };
	DEBUG_LOG(L"CustomSubExplorerCommand::Invoke menu={}, exe={}, param={}", _title, exePath, param);

	Execute(parent, exePath, param, workingDirectory);
}

void CustomSubExplorerCommand::Execute(HWND parent, const std::wstring& exePath, const std::wstring& param, const std::wstring& workingDirectory) {
	if (_run_as_flag == RunAsFlagEnum::Default) {
		ShellExecute(parent, L"open", exePath.c_str(), param.c_str(), workingDirectory.c_str(), _show_window_flag + 1);
	}
	else if (_run_as_flag == RunAsFlagEnum::RunAsAdmin) {
		ShellExecute(parent, L"runas", exePath.c_str(), param.c_str(), workingDirectory.c_str(), _show_window_flag + 1);
	}
	else if (_run_as_flag == RunAsFlagEnum::RunAsOther) {
		//TODO
	}
}
