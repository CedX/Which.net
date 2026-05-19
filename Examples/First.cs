using static Belin.Which.Finder;

// Finds the first instance of an executable.
var path = Which("foobar").First;
if (path is null) Console.Error.WriteLine("The 'foobar' command cannot be found.");
else Console.WriteLine($"The 'foobar' command is located at: {path}");
