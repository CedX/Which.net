using static Belin.Which.Finder;

// Finds all instances of an executable and returns them one at a time.
Console.WriteLine("The 'foobar' command is available at these locations:");
foreach (var path in Which("foo")) Console.WriteLine($"- {path}");
