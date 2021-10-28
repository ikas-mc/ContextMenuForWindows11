## Install vc++

winget install Microsoft.VC++2015-2019Redist-x64
or
download https://docs.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-160

## Open dev mode

Win Start > Settings > Privacy > For Developers > Developer Mode

## Install

1. extract zip
2. open powershell
3. run 
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
./Install.ps1
Set-ExecutionPolicy -ExecutionPolicy AllSigned -Scope CurrentUser

```