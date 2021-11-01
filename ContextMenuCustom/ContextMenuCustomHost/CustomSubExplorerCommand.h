#pragma once
#include "BaseExplorerCommand.h"
#include <string>
#include <winrt/base.h>
#include <winrt/Windows.Storage.h>
#include <winrt/Windows.Data.Json.h>
#include <winrt/Windows.Foundation.h>
#pragma comment(lib, "windowsapp")

class CustomSubExplorerCommand final : public BaseExplorerCommand
{
public:
	CustomSubExplorerCommand();
	CustomSubExplorerCommand(winrt::hstring const& configContent);
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) override;
	const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept override;
private:
	std::wstring _title;
	std::wstring _icon;
	std::wstring _exe;
	std::wstring _param;
};