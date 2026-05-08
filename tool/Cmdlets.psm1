using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Builds the .NET solution and all of its dependencies.
#>
function Build-DotNetSolution {
	param (
		# The configuration to use for generating the project.
		[Parameter(Position = 0)]
		[string] $Configuration
	)

	$argumentList = $Configuration ? "--configuration", $Configuration : @()
	dotnet build @argumentList
}

<#
.SYNOPSIS
	Applies style preferences and static analysis recommendations to the .NET solution.
#>
function Format-DotNetSolution {
	dotnet format
}

<#
.SYNOPSIS
	Invokes the .NET test runner.
#>
function Invoke-DotNetTest {
	param (
		# The path to the settings file to use for running the tests.
		[ValidateScript({ Test-Path $_ -PathType Leaf }, ErrorMessage = "The specified settings file does not exist.")]
		[string] $Settings
	)

	$argumentList = $Settings ? "--settings", $Settings : @()
	dotnet test @argumentList
}

<#
.SYNOPSIS
	Creates a new Git tag.
#>
function New-GitTag {
	[SuppressMessage("PSUseShouldProcessForStateChangingFunctions", "")]
	param (
		# The tag name.
		[Parameter(Mandatory, Position = 0)]
		[string] $Name
	)

	git tag $Name
	git push origin $Name
}

<#
.SYNOPSIS
	Publishes the project package to the NuGet registry.
#>
function Publish-NuGetPackage {
	param (
		# Value indicating whether to not build the solution before compression.
		[switch] $NoBuild
	)

	$output = "$PSScriptRoot/../var/NuGet"
	$argumentList = "--output", $output
	if ($NoBuild) { $argumentList += "--no-build" }
	dotnet pack @argumentList
	foreach ($package in Get-Item $output/*.nupkg) { dotnet nuget push $package --api-key $Env:NUGET_API_KEY --source NuGet }
}

<#
.SYNOPSIS
	Checks whether an update is available for the NuGet packages.
#>
function Test-NuGetPackageUpdate {
	dotnet package list --outdated
}

<#
.SYNOPSIS
	Checks whether an update is available for the specified PowerShell module.
.INPUTS
	The PowerShell module to be checked.
.OUTPUTS
	An object providing the current and the latest version of the specified module if an update is available, otherwise none.
#>
function Test-PSResourceUpdate {
	[CmdletBinding()]
	[OutputType([psobject])]
	param (
		# The PowerShell module to be checked.
		[Parameter(Mandatory, Position = 0, ValueFromPipeline)]
		[Microsoft.PowerShell.PSResourceGet.UtilClasses.PSResourceInfo] $InputObject
	)

	process {
		if ($InputObject.Repository -ne "PSGallery") { return }

		$url = "https://www.powershellgallery.com/packages/$($InputObject.Name)"
		$response = Invoke-WebRequest $url -Method Head -MaximumRedirection 0 -SkipHttpErrorCheck -ErrorAction Ignore
		$latestVersion = [semver] (Split-Path $response.Headers.Location -Leaf)

		$module = [pscustomobject]@{ ModuleName = $InputObject.Name; CurrentVersion = $InputObject.Version; LatestVersion = $latestVersion }
		if ($module.LatestVersion -gt $module.CurrentVersion) { $module }
	}
}
