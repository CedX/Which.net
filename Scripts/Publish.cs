#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

if (IsRelease) StartProcess("dotnet", "Scripts/Default.cs -Release");
else WriteError(@"The ""-Release"" switch must be set!");

WriteOutput("Tagging the repository...");
// StartProcess("git", $"tag v{Tools.Version}");
// StartProcess("git", $"push origin v{Tools.Version}");

WriteOutput("Publishing the package...");
StartProcess("dotnet", "pack --no-build --output Temp/NuGet");

var nugetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
// Directory.EnumerateFiles("Temp/NuGet", "*.nupkg")
// 	.Select(file => $@"nuget push ""{file}"" --api-key {nugetApiKey} --source NuGet")
// 	.ToList()
// 	.ForEach(arguments => StartProcess("dotnet", arguments));
