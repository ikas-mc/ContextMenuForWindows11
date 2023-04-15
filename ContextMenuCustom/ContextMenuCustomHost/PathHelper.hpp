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
class PathHelper
{
public:
	static void getExt(const std::wstring &path, bool &isDirectory, std::wstring&name, std::wstring &ext)
	{
		if (!path.empty())
		{
			std::filesystem::path file(path);
			isDirectory = is_directory(file);
			name = file.filename();
			if (!isDirectory)
			{
				ext = file.extension();
				if (!ext.empty())
				{
					std::transform(ext.begin(), ext.end(), ext.begin(), towlower); // TODO check
				}
			}
		}
	}

	static std::wstring getPath(IShellItemArray *selection)
	{
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0)
			{
				winrt::com_ptr<IShellItem> item;
				if (SUCCEEDED(selection->GetItemAt(0, item.put())))
				{
					wil::unique_cotaskmem_string path;
					item->GetDisplayName(SIGDN_FILESYSPATH, path.put());
					return std::wstring{path.get()};
				}
			}
		}
		return std::wstring{};
	}

	static std::wstring getPath(IShellItem* item)
	{
		if (item)
		{
			wil::unique_cotaskmem_string path;
			item->GetDisplayName(SIGDN_FILESYSPATH, path.put());
			return std::wstring{ path.get() };
		}
		return std::wstring{};
	}

	static std::wstring getPaths(IShellItemArray *selection, const std::wstring &delimiter)
	{
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0)
			{
				DWORD i = 0;
				std::wstringstream pathStream;
				while (i < count)
				{
					winrt::com_ptr<IShellItem> item;
					if (SUCCEEDED(selection->GetItemAt(i++, item.put())))
					{
						wil::unique_cotaskmem_string path;
						if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, path.put())))
						{
							pathStream << L'"';
							pathStream << path.get();
							pathStream << L'"';
							if (i < count)
							{
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

	static std::vector<std::wstring> getPathList(IShellItemArray *selection)
	{
		if (selection)
		{
			DWORD count;
			selection->GetCount(&count);
			if (count > 0)
			{
				std::vector<std::wstring> paths;
				DWORD i = 0;
				while (i < count)
				{
					winrt::com_ptr<IShellItem> item;
					if (SUCCEEDED(selection->GetItemAt(i++, item.put())))
					{
						wil::unique_cotaskmem_string path;
						if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, path.put())))
						{
							paths.emplace_back(path.get());
						}
					}
				}
				return paths;
			}
		}
		return std::vector<std::wstring>(0);
	}

	static void replaceAll(std::wstring &src, const std::wstring_view &from, const std::wstring &to)
	{
		if (src.length() == 0)
		{
			return;
		}

		auto fromLength = from.length();
		if (fromLength == 0)
		{
			return;
		}

		auto toLength = to.length();
		for (auto pos = src.find(from); pos != std::string::npos; pos = src.find(from, pos + toLength))
		{
			src.replace(pos, fromLength, to);
		}
	}

};
