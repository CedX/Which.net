#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

WriteOutput("Checking for outdated dependencies...");
StartProcess("dotnet", "package list --outdated");
