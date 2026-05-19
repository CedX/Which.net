"Updating the version number in the sources..."
$version = (Import-PowerShellDataFile Which.psd1).ModuleVersion
foreach ($file in Get-ChildItem -Filter *.csproj -Recurse) {
	(Get-Content $file -Raw) -replace "<Version>\d+(\.\d+){2}.*</Version>", "<Version>$version</Version>" | Set-Content $file -NoNewLine
}
