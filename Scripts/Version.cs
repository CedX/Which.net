#!/usr/bin/env dotnet
#:include Tools.cs
using System.Text.RegularExpressions;
using static Tools;

WriteOutput("Updating the version number in the sources...");
var pattern = new Regex(@"<Version>\d+(\.\d+){2}</Version>");
foreach (var file in Directory.EnumerateFiles(".", "*.csproj", SearchOption.AllDirectories))
	File.WriteAllText(file, pattern.Replace(File.ReadAllText(file), $"<Version>{Tools.Version}</Version>"));
