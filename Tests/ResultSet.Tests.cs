namespace Belin.Which;

using static Belin.Which.Finder;

/// <summary>
/// Tests the features of the <see cref="ResultSet"/> class.
/// </summary>
[TestClass]
public sealed class ResultSetTests {

	/// <summary>
	/// The path to the test fixtures.
	/// </summary>
	private readonly string fixtures = Path.GetFullPath(Path.Join(AppContext.BaseDirectory, "../Resources"));

	[TestMethod]
	public void All() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var executables = Which("Executable", paths).All;
		if (!OperatingSystem.IsWindows()) IsEmpty(executables);
		else {
			HasCount(1, executables);
			EndsWith(@"\Resources\Executable.cmd", executables[0]);
		}

		// It should return the path of the `Executable.sh` file on POSIX.
		executables = Which("Executable.sh", paths).All;
		if (OperatingSystem.IsWindows()) IsEmpty(executables);
		else {
			HasCount(1, executables);
			EndsWith("/Resources/Executable.sh", executables[0]);
		}

		// It should return an empty array if the searched command is not executable or not found.
		IsEmpty(Which("NotExecutable.sh", paths).All);
		IsEmpty(Which("foo", paths).All);
	}

	[TestMethod]
	public void First() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var executable = Which("Executable", paths).First;
		if (OperatingSystem.IsWindows()) EndsWith(@"\Resources\Executable.cmd", executable);
		else IsNull(executable);

		// It should return the path of the `Executable.sh` file on POSIX.
		executable = Which("Executable.sh", paths).First;
		if (OperatingSystem.IsWindows()) IsNull(executable);
		else EndsWith("/Resources/Executable.sh", executable);

		// It should return `null` if the searched command is not executable or not found.
		IsNull(Which("NotExecutable.sh", paths).First);
		IsNull(Which("foo", paths).First);
	}

	[TestMethod]
	public void GetEnumerator() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var found = false;
		foreach (var executable in Which("Executable", paths)) {
			EndsWith(@"\Resources\Executable.cmd", executable);
			found = true;
		}

		AreEqual(OperatingSystem.IsWindows(), found);

		// It should return the path of the `Executable.sh` file on POSIX.
		found = false;
		foreach (var executable in Which("Executable.sh", paths)) {
			EndsWith("/Resources/Executable.sh", executable);
			found = true;
		}

		AreEqual(!OperatingSystem.IsWindows(), found);

		// It should not return any result if the searched command is not executable or not found.
		found = false;
		foreach (var _ in Which("NotExecutable.sh", paths)) found = true;
		IsFalse(found);

		found = false;
		foreach (var _ in Which("foo", paths)) found = true;
		IsFalse(found);
	}
}
