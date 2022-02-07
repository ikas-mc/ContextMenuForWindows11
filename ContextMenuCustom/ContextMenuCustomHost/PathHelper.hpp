#pragma once
#include <string>
#include <vector>
#include <sstream>
#include <ShObjIdl_core.h>
#include <wil/resource.h>
#include <wil/stl.h>
#include <wil/filesystem.h>
#include <winrt/base.h>
#include <filesystem>
class PathHelper {
public:
	static void getExt(const std::wstring& path, bool& isDirectory, std::wstring& ext) {
		if (!path.empty()) {
			std::filesystem::path file(path);
			isDirectory = is_directory(file);
			if (!isDirectory) {
				ext = file.extension();
				if (!ext.empty()) {
					std::transform(ext.begin(), ext.end(), ext.begin(), towlower);//TODO check
				}
			}
		}
	}

	static std::wstring getPath(IShellItemArray* selection) {
		wil::unique_cotaskmem_string path;
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0) {
				winrt::com_ptr<IShellItem> item;
				if (SUCCEEDED(selection->GetItemAt(0, item.put()))) {
					item->GetDisplayName(SIGDN_FILESYSPATH, path.put());
				}
			}
		}
		return std::wstring{ path.get() };
	}

	static std::wstring getPaths(IShellItemArray* selection, const std::wstring& delimiter) {
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0) {
				unsigned int i = 0;
				std::wstringstream pathStream;
				while (i < count) {
					winrt::com_ptr<IShellItem> item;
					if (SUCCEEDED(selection->GetItemAt(i++, item.put()))) {
						wil::unique_cotaskmem_string  path;
						if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, path.put()))) {
							pathStream << L'"';
							pathStream << path.get();
							pathStream << L'"';
							if (i < count) {
								pathStream << delimiter;
							}
						}
					}
				}
				return pathStream.str();
			}
		}
		return std::wstring{};
	}

	static void replaceAll(std::wstring& src, const std::wstring& target, const std::wstring& repl) {
		if (src.length() == 0 || target.length() == 0) {
			return ;
		}

		size_t idx = 0;

		for (;;) {
			idx = src.find(target, idx);
			if (idx == std::wstring::npos) {
				break;
			}  

			src.replace(idx, target.length(), repl);
			idx += repl.length();
		}
	}


};
