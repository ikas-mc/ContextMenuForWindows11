#include "pch.h"
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>
#include "CustomeExplorerCommand.h"
#include "DefaultExplorerCommand.h"

#define _HAS_CXX17 1
#define _HAS_CXX20 0

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


CoCreatableClass(CustomeExplorerCommand)
CoCreatableClass(DefaultExplorerCommand)


CoCreatableClassWrlCreatorMapInclude(CustomeExplorerCommand)
CoCreatableClassWrlCreatorMapInclude(DefaultExplorerCommand)


STDAPI DllGetActivationFactory(_In_ HSTRING activatableClassId, _COM_Outptr_ IActivationFactory** factory)
{
	return Module<ModuleType::InProc>::GetModule().GetActivationFactory(activatableClassId, factory);
}

STDAPI DllCanUnloadNow()
{
	return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _COM_Outptr_ void** instance)
{
	return Module<InProc>::GetModule().GetClassObject(rclsid, riid, instance);
}