#include "pch.h"
#include "CustomeExplorerCommand.h"

const  wchar_t* CustomeExplorerCommand::Title() { return L"Custome"; };
const EXPCMDSTATE CustomeExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
const EXPCMDFLAGS CustomeExplorerCommand::Flags() { return ECF_HASSUBCOMMANDS; }


IFACEMETHODIMP CustomeExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	auto e = Make<CustomeCommands>();
	return e->QueryInterface(IID_PPV_ARGS(enumCommands));
}

const wchar_t* CustomeExplorerItemCommand::Title() { return L"CustomeExplorerItemCommand1"; }
const EXPCMDSTATE CustomeExplorerItemCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }


CustomeCommands::CustomeCommands()
{
	//TODO read from config
	m_commands.push_back(Make<CustomeExplorerItemCommand>());
	m_commands.push_back(Make<CustomeExplorerItemCommand>());
	m_current = m_commands.cbegin();
}
IFACEMETHODIMP CustomeCommands::Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
{
	ULONG fetched{ 0 };
	wil::assign_to_opt_param(pceltFetched, 0ul);

	for (ULONG i = 0; (i < celt) && (m_current != m_commands.cend()); i++)
	{
		m_current->CopyTo(&apUICommand[0]);
		m_current++;
		fetched++;
	}

	wil::assign_to_opt_param(pceltFetched, fetched);
	return (fetched == celt) ? S_OK : S_FALSE;
}
IFACEMETHODIMP CustomeCommands::Skip(ULONG /*celt*/) { return E_NOTIMPL; }
IFACEMETHODIMP CustomeCommands::Reset()
{
	m_current = m_commands.cbegin();
	return S_OK;
}
IFACEMETHODIMP CustomeCommands::Clone(__deref_out IEnumExplorerCommand** ppenum) { *ppenum = nullptr; return E_NOTIMPL; }

