#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

var release = IsRelease ? "-Release" : "";
Array.ForEach(["Clean", "Version", "Build"], file => StartProcess("dotnet", $"Scripts/{file}.cs {release}".TrimEnd()));
