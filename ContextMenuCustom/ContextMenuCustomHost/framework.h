#pragma once
#define WIN32_LEAN_AND_MEAN
#include <wil/cppwinrt.h>
#include <wil/stl.h>
#include <wil/com.h>
#include <wil/win32_helpers.h>
#include <winrt/Windows.Storage.h>
#include <winrt/Windows.Data.Json.h>
#include <winrt/Windows.Foundation.h>
#include <winrt/Windows.Foundation.Collections.h>

#include <ShObjIdl_core.h>
#include <Shlwapi.h>
#include <shellapi.h>
#include <ShlObj.h>

#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "runtimeobject.lib")
#pragma comment(lib, "windowsapp")