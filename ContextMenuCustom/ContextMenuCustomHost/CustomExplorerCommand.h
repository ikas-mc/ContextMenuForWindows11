#pragma once
#include "BaseExplorerCommand.h"
#include <string>

class __declspec(uuid("46F650E5-9959-48D6-AC13-A9637C5B3787"))   CustomExplorerCommand : public BaseExplorerCommand
{
public:
	CustomExplorerCommand();
	const  wchar_t* GetIconId();
	const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override;
	const EXPCMDFLAGS Flags() override;
	IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name) override;
	IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) override;

private:
	std::wstring m_current_path;
};


class CustomCommands : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
	CustomCommands();
	void ReadCommands(std::wstring & current_path);
	IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched);
	IFACEMETHODIMP Skip(ULONG /*celt*/);
	IFACEMETHODIMP Reset();
	IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum);

private:
	std::vector<ComPtr<IExplorerCommand>> m_commands;
	std::vector<ComPtr<IExplorerCommand>>::const_iterator m_current;
};
