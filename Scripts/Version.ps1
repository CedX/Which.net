using module ./Cmdlets.psm1

"Updating the version number in the sources..."
$version = Get-Version
Get-ChildItem -File -Filter *.csproj -Recurse | ForEach-Object {
	(Get-Content $_ -Raw) -replace "<Version>\d+(\.\d+){2}.*</Version>", "<Version>$version</Version>" | Set-Content $_ -NoNewLine
}
