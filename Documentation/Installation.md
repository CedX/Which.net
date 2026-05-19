# Installation

## Requirements
Before installing **Which for .NET**, you need to make sure you have the [.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/sdk)
and the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools) tool up and running.
		
You can verify if you're already good to go with the following command:

```shell
dotnet --version
# 10.0.201
```

## Installing with NuGet package manager

### 1. Install it
From a command prompt, run:

```shell
dotnet package add Belin.Which
```

### 2. Import it
Now in your [C#](https://learn.microsoft.com/en-us/dotnet/csharp) code, you can use:

```cs
using static Belin.Which.Finder;
```
