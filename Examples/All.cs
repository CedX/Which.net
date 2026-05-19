using static Belin.Which.Finder;

// Finds all instances of an executable.
var paths = Which("foobar").All;
if (paths.Length == 0) Console.Error.WriteLine("The 'foobar' command cannot be found.");
else {
	Console.WriteLine("The 'foobar' command is available at these locations:");
	foreach (var path in paths) Console.WriteLine($"- {path}");
}
