#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

WriteOutput("Building the solution...");
StartProcess("dotnet", $"build --configuration {Configuration}");
