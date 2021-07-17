#include "pch.h"
#include "DefaultExplorerCommand.h"
#include "VsCodeExplorerCommand.h"
#include "SzExplorerCommand.h"
#include "CustomeExplorerCommand.h"

const  wchar_t* DefaultExplorerCommand::Title() { return L"More Menu"; };
const EXPCMDSTATE DefaultExplorerCommand::State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; };
const EXPCMDFLAGS DefaultExplorerCommand::Flags() { return ECF_HASSUBCOMMANDS; }


IFACEMETHODIMP DefaultExplorerCommand::EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
{
	*enumCommands = nullptr;
	auto e = Make<DefaultCommands>();
	return e->QueryInterface(IID_PPV_ARGS(enumCommands));
}

DefaultCommands::DefaultCommands()
{
	m_commands.push_back(Make<VsCodeExplorerCommand>());
	m_commands.push_back(Make<SzExplorerCommand>());
	//m_commands.push_back(Make<CustomeExplorerCommand>());
	m_current = m_commands.cbegin();
}

IFACEMETHODIMP DefaultCommands::Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
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
IFACEMETHODIMP DefaultCommands::Skip(ULONG /*celt*/) { return E_NOTIMPL; }
IFACEMETHODIMP DefaultCommands::Reset()
{
	m_current = m_commands.cbegin();
	return S_OK;
}
IFACEMETHODIMP DefaultCommands::Clone(__deref_out IEnumExplorerCommand** ppenum) { *ppenum = nullptr; return E_NOTIMPL; }

