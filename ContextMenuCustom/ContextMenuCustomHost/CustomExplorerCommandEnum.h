#pragma once
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <shobjidl_core.h>
#include <string>
#include <vector>
#include <shellapi.h>
#include <shlwapi.h>

using namespace Microsoft::WRL;
class CustomExplorerCommandEnum : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
	CustomExplorerCommandEnum(std::vector<ComPtr<IExplorerCommand>>& commands);
	IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched);
	IFACEMETHODIMP Skip(ULONG /*celt*/);
	IFACEMETHODIMP Reset();
	IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum);
private:
	std::vector<ComPtr<IExplorerCommand>> m_commands;
	size_t m_current;
};
