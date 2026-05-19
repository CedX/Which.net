namespace Belin.Which;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests the features of the <see cref="Finder"/> class.
/// </summary>
[TestClass]
public sealed class FinderTests {

	/// <summary>
	/// The path to the test fixtures.
	/// </summary>
	private readonly string fixtures = Path.GetFullPath(Path.Join(AppContext.BaseDirectory, "../Resources"));

	[TestMethod]
	public void Constructor() {
		var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

		// It should set the `Paths` property to the value of the `PATH` environment variable by default.
		var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
		List<string> paths = pathEnv.Length > 0 ? [.. pathEnv.Split(Path.PathSeparator, splitOptions).Distinct()] : [];
		CollectionAssert.AreEqual(paths, new Finder().Paths.ToList());

		// It should set the `Extensions` property to the value of the `PATHEXT` environment variable by default.
		var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? "";
		List<string> extensions = pathExt.Length > 0 ? [.. pathExt.Split(';', splitOptions).Select(item => item.ToLowerInvariant()).Distinct()] : [".exe", ".cmd", ".bat", ".com"];
		CollectionAssert.AreEqual(extensions, new Finder().Extensions.ToList());

		// It should put in lower case the list of file extensions.
		CollectionAssert.AreEqual(new List<string> { ".exe", ".js", ".ps1" }, new Finder(extensions: [".EXE", ".JS", ".PS1"]).Extensions.ToList());
	}

	[TestMethod]
	public void Find() {
		var finder = new Finder(paths: [fixtures]);

		// It should return the path of the `Executable.cmd` file on Windows.
		List<string> executables = [.. finder.Find("Executable")];
		HasCount(OperatingSystem.IsWindows() ? 1 : 0, executables);
		if (OperatingSystem.IsWindows()) EndsWith(@"Resources\Executable.cmd", executables.First());

		// It should return the path of the `Executable.sh` file on POSIX.
		executables = [.. finder.Find("Executable.sh")];
		HasCount(OperatingSystem.IsWindows() ? 0 : 1, executables);
		if (!OperatingSystem.IsWindows()) EndsWith("Resources/Executable.sh", executables.First());

		// It should return an empty array if the searched command is not executable or not found.
		IsEmpty(finder.Find("NotExecutable.sh"));
		IsEmpty(finder.Find("foo"));
	}

	[TestMethod]
	public void IsExecutable() {
		var finder = new Finder();

		// It should return `false` if the searched command is not executable or not found.
		IsFalse(finder.IsExecutable("foo/bar/baz.qux"));
		IsFalse(finder.IsExecutable("Resources/NotExecutable.sh"));

		// It should return `false` for a POSIX executable, when test is run on Windows.
		AreEqual(!OperatingSystem.IsWindows(), finder.IsExecutable(Path.Join(fixtures, "Executable.sh")));

		// It should return `false` for a Windows executable, when test is run on POSIX.
		AreEqual(OperatingSystem.IsWindows(), finder.IsExecutable(Path.Join(fixtures, "Executable.cmd")));
	}
}
