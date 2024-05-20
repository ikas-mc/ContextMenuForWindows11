#include "pch.h"
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include "CustomExplorerCommand.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		DisableThreadLibraryCalls(hModule);
		break;
	}
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
	{
		break;
	}
	}
	return TRUE;
}

CoCreatableClass(CustomExplorerCommand);
CoCreatableClassWrlCreatorMapInclude(CustomExplorerCommand);

STDAPI DllCanUnloadNow()
{
	return Module<InProc>::GetModule().Terminate() ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID FAR *ppv)
{
#if defined(CMC_ANY)
	return Module<InProc>::GetModule().GetClassObject(__uuidof(CustomExplorerCommand), riid, ppv);
#else
	return Module<InProc>::GetModule().GetClassObject(rclsid, riid, ppv);
#endif
}