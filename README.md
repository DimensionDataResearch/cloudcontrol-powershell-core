# CloudControl module for PowerShell Core

## Introduction

A Powershell Core module for interacting with Dimension Data CloudControl.
It's functional, but definitely still a work in progress.

## Requirements

Needs PowerShell Core v6.0.0-alpha17 or newer.

## Getting started
 
To get started:

```bash
dotnet restore lib/cloudcontrol-client-core
dotnet build lib/cloudcontrol-client-core

dotnet restore
dotnet build
dotnet publish -c release -o $PWD/bin/release
```

Then, in powershell:

```powershell
Import-Module './bin/release/CloudControl.psd1'
New-CloudControlConnection -Name 'Australia' -Region 'AU' -UserName 'my_mcp_username' -Password 'my_mcp_password' -SetDefault

Get-CloudControlUserAccount -My
```
