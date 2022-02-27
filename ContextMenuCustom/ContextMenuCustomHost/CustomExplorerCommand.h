#pragma once
#include "BaseExplorerCommand.h"
#include <string>

class __declspec(uuid("46F650E5-9959-48D6-AC13-A9637C5B3787"))   CustomExplorerCommand : public BaseExplorerCommand
{
public:
	CustomExplorerCommand();
	const  wchar_t* GetIconId();
	const EXPCMDFLAGS Flags() override;
	IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState) override;
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) override;
	IFACEMETHODIMP EnumSubCommands(__RPC__deref_out_opt IEnumExplorerCommand** enumCommands) override;
	void ReadCommands(bool multipeFiles, const std::wstring& current_path);

private:
	std::vector<ComPtr<IExplorerCommand>> m_commands;
};
