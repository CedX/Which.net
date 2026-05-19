# Which for .NET
Find the instances of an executable in the system path,
in [C#](https://learn.microsoft.com/en-us/dotnet/csharp).

## Quick start
Install the latest version of **Which for .NET** with [NuGet](https://www.nuget.org) package manager:

```shell
dotnet package add Belin.Which
```

For detailed instructions, see the [installation guide](Installation.md).

## Usage
This package provides the `Finder.Which(string command)` method, allowing to locate a command in the system path.  
This method takes the name of the command to locate, and returns a `ResultSet` instance.

The `ResultSet` class implements the `IEnumerable<string>` interface.  
It is therefore possible to iterate over the results using a `foreach` loop:

```cs
using static Belin.Which.Finder;

// Finds all instances of an executable and returns them one at a time.
Console.WriteLine("The 'foobar' command is available at these locations:");
foreach (var path in Which("foo")) Console.WriteLine($"- {path}");
```

The `ResultSet` class also provides two convenient properties:

- `All` : get all instances of the searched command.
- `First` : get the first instance of the searched command.

### string[] **All**
The `ResultSet.All` property returns an array of the absolute paths of all instances of an executable found in the system path.
If the executable could not be located, it returns an empty array.

```cs
using static Belin.Which.Finder;

var paths = Which("foobar").All;
if (paths.Length == 0) Console.Error.WriteLine("The 'foobar' command cannot be found.");
else {
  Console.WriteLine("The 'foobar' command is available at these locations:");
  foreach (var path in paths) Console.WriteLine($"- {path}");
}
```

### string? **First**
The `ResultSet.First` property returns the absolute path of the first instance of an executable found in the system path.
If the executable could not be located, it returns a `null` reference.

```cs
using static Belin.Which.Finder;

var path = Which("foobar").First;
if (path is null) Console.Error.WriteLine("The 'foobar' command cannot be found.");
else Console.WriteLine($"The 'foobar' command is located at: {path}");
```

## Options
The behavior of the `Finder.Which(string command, IEnumerable<string>? paths = null, IEnumerable<string>? extensions = null)` method can be customized using the following parameters.

### IEnumerable&lt;string&gt; **extensions**
An enumerable of strings specifying the list of executable file extensions.
On Windows, defaults to the list of extensions provided by the `PATHEXT` environment variable.

```cs
Which("foobar", extensions: [".foo", ".exe", ".cmd"]);
```

> [!NOTE]
> The `extensions` option is only meaningful on the Windows platform,
> where the executability of a file is determined from its extension.

### IEnumerable&lt;string&gt; **paths**
An enumerable of strings specifying the system paths from which the given command will be searched.
Defaults to the list of directories provided by the `PATH` environment variable.

```cs
Which("foobar", paths: ["/usr/local/bin", "/usr/bin"]);
```
