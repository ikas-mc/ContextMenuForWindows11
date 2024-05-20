#pragma once
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include <vector>
#include "CustomSubExplorerCommand.h"

using namespace Microsoft::WRL;

class CustomExplorerCommandEnum final : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand> {
public:
	explicit CustomExplorerCommandEnum(std::vector<ComPtr<CustomSubExplorerCommand>>& commands);
	IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched) override;
	IFACEMETHODIMP Skip(ULONG /*celt*/) override;
	IFACEMETHODIMP Reset() override;
	IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum) override;

private:
	std::vector<ComPtr<CustomSubExplorerCommand>> m_commands;
	size_t m_current;
};
