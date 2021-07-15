#pragma once
#include "BaseExplorerCommand.h"
class CustomeExplorerCommand : public BaseExplorerCommand
{
	const  wchar_t* Title();
	const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override;
	const EXPCMDFLAGS Flags() override;
	IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) override;
};


class CustomeCommands : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
	CustomeCommands();
	IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched);
	IFACEMETHODIMP Skip(ULONG /*celt*/);
	IFACEMETHODIMP Reset();
	IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum);

private:
	std::vector<ComPtr<IExplorerCommand>> m_commands;
	std::vector<ComPtr<IExplorerCommand>>::const_iterator m_current;
};


class CustomeExplorerItemCommand final : public BaseExplorerCommand
{
public:
	const wchar_t* Title() override;
	const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) override;
};