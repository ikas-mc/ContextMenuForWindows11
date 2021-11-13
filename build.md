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

