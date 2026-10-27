#!/usr/bin/env pwsh
param (
	# The script to invoke.
	[Parameter(Position = 1)]
	[ArgumentCompleter({
		param ([string] $commandName, [string] $parameterName, [string] $wordToComplete)
		(Get-Item "$PSScriptRoot/Scripts/$wordToComplete*.cs").BaseName
	})]
	[ValidateScript(
		{ Test-Path "$PSScriptRoot/Scripts/$_.cs" -PathType Leaf },
		ErrorMessage = "The specified script does not exist."
	)]
	[string] $Script = "Default",

	# The script arguments.
	[Parameter(ValueFromRemainingArguments)]
	[string[]] $Arguments = @()
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
dotnet "$PSScriptRoot/Scripts/$Script.cs" @Arguments
