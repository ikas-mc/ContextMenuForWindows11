#include "pch.h"
#include "CustomExplorerCommandEnum.h"
#include <wil/common.h>


CustomExplorerCommandEnum::CustomExplorerCommandEnum(std::vector<ComPtr<CustomSubExplorerCommand>>& commands):m_current(0) {
	m_commands = commands;
}


IFACEMETHODIMP CustomExplorerCommandEnum::Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
{
	ULONG fetched{ 0 };
	wil::assign_to_opt_param(pceltFetched, 0ul);

	auto size = m_commands.size();
	if (m_current < size) {
		auto current = m_commands.cbegin();
		current += m_current;
		for (; fetched < celt && m_current < size;)
		{
			current->CopyTo(&apUICommand[0]);
			current++;
			m_current++;
			fetched++;
		}
	}

	wil::assign_to_opt_param(pceltFetched, fetched);
	return (fetched == celt) ? S_OK : S_FALSE;
}
IFACEMETHODIMP CustomExplorerCommandEnum::Skip(ULONG celt) {
	if ((m_current + static_cast<int>(celt)) >= m_commands.size()) {
		return S_FALSE;
	}
	m_current += celt;
	return S_OK;
}

IFACEMETHODIMP CustomExplorerCommandEnum::Reset()
{
	m_current = 0;
	return S_OK;
}
IFACEMETHODIMP CustomExplorerCommandEnum::Clone(__deref_out IEnumExplorerCommand** ppenum) { 
	if (ppenum == nullptr) {
		return E_POINTER;
	}

	auto newEnum = Make<CustomExplorerCommandEnum>(m_commands);
	newEnum->m_current = m_current;
	return newEnum->QueryInterface(IID_PPV_ARGS(ppenum));
}
