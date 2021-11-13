#pragma once
#include "BaseExplorerCommand.h"
#include <string>
#include <winrt/base.h>
#include <winrt/Windows.Storage.h>
#include <winrt/Windows.Data.Json.h>
#include <winrt/Windows.Foundation.h>

class CustomSubExplorerCommand final : public BaseExplorerCommand
{
public:
	CustomSubExplorerCommand(winrt::hstring const& configContent);
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) override;
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept override;
	virtual bool Accept(bool isDirectory, std::wstring& ext);
private:
	std::wstring _title;
	std::wstring _icon;
	std::wstring _exe;
	std::wstring _param;
	bool _accept_directory = false;
	std::wstring _accept_exts;
};