namespace Belin.Which;

using static Belin.Which.Finder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
		if (!OperatingSystem.IsWindows()) Assert.IsEmpty(executables);
		else {
			Assert.HasCount(1, executables);
			Assert.EndsWith(@"\Resources\Executable.cmd", executables[0]);
		}

		// It should return the path of the `Executable.sh` file on POSIX.
		executables = Which("Executable.sh", paths).All;
		if (OperatingSystem.IsWindows()) Assert.IsEmpty(executables);
		else {
			Assert.HasCount(1, executables);
			Assert.EndsWith("/Resources/Executable.sh", executables[0]);
		}

		// It should return an empty array if the searched command is not executable or not found.
		Assert.IsEmpty(Which("NotExecutable.sh", paths).All);
		Assert.IsEmpty(Which("foo", paths).All);
	}

	[TestMethod]
	public void First() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var executable = Which("Executable", paths).First;
		if (OperatingSystem.IsWindows()) Assert.EndsWith(@"\Resources\Executable.cmd", executable);
		else Assert.IsNull(executable);

		// It should return the path of the `Executable.sh` file on POSIX.
		executable = Which("Executable.sh", paths).First;
		if (OperatingSystem.IsWindows()) Assert.IsNull(executable);
		else Assert.EndsWith("/Resources/Executable.sh", executable);

		// It should return `null` if the searched command is not executable or not found.
		Assert.IsNull(Which("NotExecutable.sh", paths).First);
		Assert.IsNull(Which("foo", paths).First);
	}

	[TestMethod]
	public void GetEnumerator() {
		var paths = new string[] { fixtures };

		// It should return the path of the `Executable.cmd` file on Windows.
		var found = false;
		foreach (var executable in Which("Executable", paths)) {
			Assert.EndsWith(@"\Resources\Executable.cmd", executable);
			found = true;
		}

		Assert.AreEqual(OperatingSystem.IsWindows(), found);

		// It should return the path of the `Executable.sh` file on POSIX.
		found = false;
		foreach (var executable in Which("Executable.sh", paths)) {
			Assert.EndsWith("/Resources/Executable.sh", executable);
			found = true;
		}

		Assert.AreEqual(!OperatingSystem.IsWindows(), found);

		// It should not return any result if the searched command is not executable or not found.
		found = false;
		foreach (var _ in Which("NotExecutable.sh", paths)) found = true;
		Assert.IsFalse(found);

		found = false;
		foreach (var _ in Which("foo", paths)) found = true;
		Assert.IsFalse(found);
	}
}
