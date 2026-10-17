using module ./Cmdlets.psm1

if ($Release) { & "$PSScriptRoot/Default.ps1" }
else { "The ""-Release"" switch must be set!"; exit 1 }

"Publishing the package..."
New-GitTag "v$(Get-Version)"
Publish-NuGetPackage -NoBuild
