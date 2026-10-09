namespace Belin.Which;

using static Belin.Which.Finder;

/// <summary>
/// Tests the features of the <see cref="ResultSet"/> class.
/// </summary>
[TestClass]
public class ResultSetTests {

	/// <summary>
	/// The path to the test fixtures.
	/// </summary>
	private readonly string fixtures = Path.GetFullPath(Path.Join(AppContext.BaseDirectory, "../Resources"));

	[TestMethod]
	public void All() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var executables = Which("Executable", paths).All;
		if (!OperatingSystem.IsWindows()) executables.ShouldBeEmpty();
		else {
			executables.Length.ShouldBe(1);
			executables[0].ShouldEndWith(@"\Resources\Executable.cmd");
		}

		// It should return the path of the `Executable.sh` file on POSIX.
		executables = Which("Executable.sh", paths).All;
		if (OperatingSystem.IsWindows()) executables.ShouldBeEmpty();
		else {
			executables.Length.ShouldBe(1);
			executables[0].ShouldEndWith("/Resources/Executable.sh");
		}

		// It should return an empty array if the searched command is not executable or not found.
		Which("NotExecutable.sh", paths).All.ShouldBeEmpty();
		Which("foo", paths).All.ShouldBeEmpty();
	}

	[TestMethod]
	public void First() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var executable = Which("Executable", paths).First;
		if (OperatingSystem.IsWindows()) executable.ShouldEndWith(@"\Resources\Executable.cmd");
		else executable.ShouldBeNull();

		// It should return the path of the `Executable.sh` file on POSIX.
		executable = Which("Executable.sh", paths).First;
		if (OperatingSystem.IsWindows()) executable.ShouldBeNull();
		else executable.ShouldEndWith("/Resources/Executable.sh");

		// It should return `null` if the searched command is not executable or not found.
		Which("NotExecutable.sh", paths).First.ShouldBeNull();
		Which("foo", paths).First.ShouldBeNull();
	}

	[TestMethod]
	public void GetEnumerator() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var found = false;
		foreach (var executable in Which("Executable", paths)) {
			executable.ShouldEndWith(@"\Resources\Executable.cmd");
			found = true;
		}

		found.ShouldBe(OperatingSystem.IsWindows());

		// It should return the path of the `Executable.sh` file on POSIX.
		found = false;
		foreach (var executable in Which("Executable.sh", paths)) {
			executable.ShouldEndWith("/Resources/Executable.sh");
			found = true;
		}

		found.ShouldNotBe(OperatingSystem.IsWindows());

		// It should not return any result if the searched command is not executable or not found.
		found = false;
		foreach (var _ in Which("NotExecutable.sh", paths)) found = true;
		found.ShouldBeFalse();

		found = false;
		foreach (var _ in Which("foo", paths)) found = true;
		found.ShouldBeFalse();
	}
}
