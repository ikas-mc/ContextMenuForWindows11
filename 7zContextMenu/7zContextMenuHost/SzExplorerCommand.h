#pragma once
#include "BaseExplorerCommand.h"
class SzExplorerCommand final : public BaseExplorerCommand
{
	const  wchar_t* Title();
	const EXPCMDSTATE State  (_In_opt_ IShellItemArray* selection) override;
	IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept override;
};