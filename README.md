# compute-powershell-core

A Powershell Core module for interacting with Dimension Data CloudControl.
It's functional, but definitely still a work in progress.
 
To get started:

```bash
dotnet restore lib/cloudcontrol-client-core
dotnet build lib/cloudcontrol-client-core

dotnet restore
dotnet build
dotnet publish
```

Then, in powershell:

```powershell
Import-Module './src/DD.CloudControl.Powershell/bin/Debug/netstandard1.6/publish/DD.CloudControl.Powershell.dll'
New-CloudControlConnection -Name 'Australia' -Region 'AU' -UserName 'my_mcp_username' -Password 'my_mcp_password' -SetDefault

Get-CloudControlUserAccount -My
```
