namespace Belin.Which;

/// <summary>
/// Tests the features of the <see cref="Finder"/> class.
/// </summary>
[TestClass]
public class FinderTests {

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
		new Finder().Paths.ShouldBe(paths);

		// It should set the `Extensions` property to the value of the `PATHEXT` environment variable by default.
		var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? "";
		List<string> extensions = pathExt.Length > 0 ? [.. pathExt.Split(';', splitOptions).Select(item => item.ToLowerInvariant()).Distinct()] : [".exe", ".cmd", ".bat", ".com"];
		new Finder().Extensions.ShouldBe(extensions);

		// It should put in lower case the list of file extensions.
		new Finder(extensions: [".EXE", ".JS", ".PS1"]).Extensions.ShouldBe([".exe", ".js", ".ps1"]);
	}

	[TestMethod]
	public void Find() {
		var finder = new Finder(paths: [fixtures]);

		// It should return the path of the `Executable.cmd` file on Windows.
		List<string> executables = [.. finder.Find("Executable")];
		executables.Count.ShouldBe(OperatingSystem.IsWindows() ? 1 : 0);
		if (OperatingSystem.IsWindows()) executables.First().ShouldEndWith(@"Resources\Executable.cmd");

		// It should return the path of the `Executable.sh` file on POSIX.
		executables = [.. finder.Find("Executable.sh")];
		executables.Count.ShouldBe(OperatingSystem.IsWindows() ? 0 : 1);
		if (!OperatingSystem.IsWindows()) executables.First().ShouldEndWith("Resources/Executable.sh");

		// It should return an empty array if the searched command is not executable or not found.
		finder.Find("NotExecutable.sh").ShouldBeEmpty();
		finder.Find("foo").ShouldBeEmpty();
	}

	[TestMethod]
	public void IsExecutable() {
		var finder = new Finder();

		// It should return `false` if the searched command is not executable or not found.
		finder.IsExecutable("foo/bar/baz.qux").ShouldBeFalse();
		finder.IsExecutable("Resources/NotExecutable.sh").ShouldBeFalse();

		// It should return `false` for a POSIX executable, when test is run on Windows.
		finder.IsExecutable(Path.Join(fixtures, "Executable.sh")).ShouldNotBe(OperatingSystem.IsWindows());

		// It should return `false` for a Windows executable, when test is run on POSIX.
		finder.IsExecutable(Path.Join(fixtures, "Executable.cmd")).ShouldBe(OperatingSystem.IsWindows());
	}
}
