#!/usr/bin/env dotnet
#:include Tools.cs
using static Tools;

WriteOutput("Deleting all generated files...");
Directory.EnumerateDirectories(".", "obj", SearchOption.AllDirectories).ToList().ForEach(RemoveItem);
RemoveItem("Binaries");
ClearItem("Temp");
