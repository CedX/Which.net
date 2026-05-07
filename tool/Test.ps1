using module ./Cmdlets.psm1

"Running the test suite..."
Invoke-DotNetTest -Settings .runsettings
