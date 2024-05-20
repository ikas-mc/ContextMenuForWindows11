#pragma once
#include "BaseExplorerCommand.h"
#include <string>

enum FilesMatchFlagEnum {
	FILES_OFF = 0,
	FILES_EACH = 1,
	FILES_JOIN = 2
};

enum FileMatchFlagEnum {
	FILE_OFF = 0,
	FILE_EXT = 1,
	FILE_REGEX = 2,
	FILE_EXT2 = 3,
	FILE_ALL = 4
};

enum DirectoryMatchFlagEnum {
	DIRECTORY_OFF = 0,
	DIRECTORY_DIRECTORY = 0b0001,
	DIRECTORY_BACKGROUND = 0b0010,
	DIRECTORY_DESKTOP = 0b0100
};

constexpr std::wstring_view PARAM_PATH = L"{path}";
constexpr std::wstring_view PARAM_NAME = L"{name}";
constexpr std::wstring_view PARAM_EXT = L"{extension}";
constexpr std::wstring_view PARAM_PARENT = L"{parent}";
constexpr std::wstring_view PARAM_NAME_NO_EXT = L"{nameNoExt}";

class CustomSubExplorerCommand final : public BaseExplorerCommand {
public:
	CustomSubExplorerCommand(const winrt::hstring& configContent, ThemeType themeType,bool enableEnbug);
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) override;
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept override;
	virtual bool Accept(bool multipleFiles, FileType fileType, const std::wstring& name, const std::wstring& ext);

private:
	void Execute(HWND parent, const std::wstring& path);
	std::wstring _exe;
	std::wstring _param;
	bool _accept_directory;
	bool _accept_file;
	std::wstring _accept_exts;
	int _accept_multiple_files_flag;
	std::wstring _path_delimiter;
	std::wstring _param_for_multiple_files;
	std::wstring _icon;
	std::wstring _icon_dark;
	std::wstring _title;
	std::wstring _accept_file_regex;
	int _accept_file_flag;
	int _accept_directory_flag;

public:
	int m_index;
};
