#pragma once
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <shobjidl_core.h>
#include <string>
#include <vector>
#include <shellapi.h>
#include <shlwapi.h>
#include "CustomSubExplorerCommand.h"

using namespace Microsoft::WRL;
class CustomExplorerCommandEnum : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
	CustomExplorerCommandEnum(std::vector<ComPtr<CustomSubExplorerCommand>>& commands);
	IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched);
	IFACEMETHODIMP Skip(ULONG /*celt*/);
	IFACEMETHODIMP Reset();
	IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum);
private:
	std::vector<ComPtr<CustomSubExplorerCommand>> m_commands;
	size_t m_current;
};
