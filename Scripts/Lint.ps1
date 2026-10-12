using module PSScriptAnalyzer

"Performing the static analysis of source code..."
Invoke-ScriptAnalyzer $PSScriptRoot -Recurse
