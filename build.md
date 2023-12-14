# how to build

## build ContextMenuCustomHost
1. install nuget package: Microsoft.Windows.CppWinRT and Microsoft.Windows.ImplementationLibrary
2. clean project, select debug or release,repeat this step every time you modify the configuration   
(you can change the output to ContextMenuCustomApp debug or release bin folder)

## build ContextMenuCustomApp
1. change PackageCertificateKey
2. build

## build ContextMenuCustomPackage

1. change PackageCertificateKey
2. set this project as startup project
3. build and run

# publish
1. clean all
2. select release,rebuild ContextMenuCustomHost project
3. publish on ContextMenuCustomPackage project



## some helpful links 


https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-extensions#context-menu

https://github.com/MicrosoftDocs/winrt-related/blob/docs/winrt-related-src/schemas/appxpackage/uapmanifestschema/element-desktop5-itemtype.md

https://github.com/microsoft/Windows-classic-samples/tree/master/Samples/Win7Samples/winui/shell/appshellintegration/ExplorerCommandVerb

https://github.com/microsoft/AppModelSamples/tree/0f88fe0a8bbb90051f6c126741b881288bb5cabb/Samples/SparsePackages/PhotoStoreContextMenu

https://github.com/microsoft/terminal/tree/fb597ed304ec6eef245405c9652e9b8a029b821f/src/cascadia/ShellExtension

https://github.com/MicrosoftDocs/windows-uwp/blob/docs/hub/apps/desktop/modernize/integrate-packaged-app-with-file-explorer.md

https://docs.microsoft.com/en-us/cpp/cppcx/wrl/windows-runtime-cpp-template-library-wrl?view=msvc-170
