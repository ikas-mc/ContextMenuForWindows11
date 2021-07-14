# Context Menu For Windows11

Demos  For Add Context Menu

* 7zContextMenu : add [7z Extract Here] Menu




```xml

    <desktop4:Extension Category="windows.fileExplorerContextMenus">
          <desktop4:FileExplorerContextMenus>
            <desktop5:ItemType Type="*"  >
              <desktop5:Verb Id="Command1" Clsid="46F650E5-9959-48D6-AC13-A9637C5B3787" />
            </desktop5:ItemType>
          </desktop4:FileExplorerContextMenus>
        </desktop4:Extension>
        <com:Extension Category="windows.comServer">
          <com:ComServer>
            <com:SurrogateServer  DisplayName="Context menu verb handler">
              <com:Class Id="46F650E5-9959-48D6-AC13-A9637C5B3787" Path="7zContextMenu2\7zContextMenuHost.dll" ThreadingModel="STA"/>
            </com:SurrogateServer>
          </com:ComServer>
        </com:Extension>

```


Install

```powershell
0.
winget install  Microsoft.VC++2015-2019Redist-x64

1.
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

2.
./Install.ps1

3.
Set-ExecutionPolicy -ExecutionPolicy AllSigned -Scope CurrentUser

```

more  
https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-extensions#context-menu

https://github.com/MicrosoftDocs/winrt-related/blob/docs/winrt-related-src/schemas/appxpackage/uapmanifestschema/element-desktop5-itemtype.md

https://github.com/microsoft/Windows-classic-samples/tree/master/Samples/Win7Samples/winui/shell/appshellintegration/ExplorerCommandVerb

https://github.com/microsoft/AppModelSamples/tree/0f88fe0a8bbb90051f6c126741b881288bb5cabb/Samples/SparsePackages/PhotoStoreContextMenu
