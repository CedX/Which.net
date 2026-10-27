#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

WriteOutput("Running the test suite...");
StartProcess("dotnet", "test --settings .runsettings");
