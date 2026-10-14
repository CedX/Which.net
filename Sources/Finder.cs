namespace Belin.Which;

using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

/// <summary>
/// Finds the instances of an executable in the system path.
/// </summary>
public partial class Finder {

	/// <summary>
	/// Gets the regular expression used to remove quotation marks from a path.
	/// </summary>
	/// <returns>The regular expression used to remove quotation marks from a path.</returns>
	[GeneratedRegex(@"^""|""$")]
	private static partial Regex QuotePattern();

	/// <summary>
	/// The list of executable file extensions.
	/// </summary>
	public IList<string> Extensions {
		get; set => field = [.. value.Select(item => item.ToLowerInvariant()).Distinct()];
	}

	/// <summary>
	/// The list of system paths.
	/// </summary>
	public IList<string> Paths {
		get; set {
			var quotePattern = QuotePattern();
			field = [.. value.Select(item => quotePattern.Replace(item, "")).Distinct()];
		}
	}

	/// <summary>
	/// Creates a new finder.
	/// </summary>
	/// <param name="paths">The list of system paths.</param>
	/// <param name="extensions">The list of executable file extensions.</param>
	public Finder(string[]? paths = null, string[]? extensions = null) {
		var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

		paths ??= [];
		if (paths.Length == 0) {
			var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
			paths = pathEnv.Length > 0 ? pathEnv.Split(Path.PathSeparator, splitOptions) : [];
		}

		extensions ??= [];
		if (extensions.Length == 0) {
			var pathExt = Environment.GetEnvironmentVariable("PATHEXT") ?? "";
			extensions = pathExt.Length > 0 ? pathExt.Split(';', splitOptions) : [".exe", ".cmd", ".bat", ".com"];
		}

		Extensions = extensions;
		Paths = paths;
	}

	/// <summary>
	/// Finds the instances of the specified command in the system path.
	/// </summary>
	/// <param name="command">The command to be resolved.</param>
	/// <param name="paths">The system path. Defaults to the <c>PATH</c> environment variable.</param>
	/// <param name="extensions">The executable file extensions. Defaults to the <c>PATHEXT</c> environment variable.</param>
	/// <returns>The search results.</returns>
	public static ResultSet Which(string command, string[]? paths = null, string[]? extensions = null) =>
		new(command, new Finder(paths, extensions));

	/// <summary>
	/// Finds the instances of an executable in the system path.
	/// </summary>
	/// <param name="command">The command to be resolved.</param>
	/// <returns>The paths of the executables found.</returns>
	public IEnumerable<string> Find(string command) => Paths.SelectMany(directory => FindExecutables(directory, command));

	/// <summary>
	/// Gets a value indicating whether the specified file is executable.
	/// </summary>
	/// <param name="file">The file to be checked.</param>
	/// <returns><see langword="true"/> if the specified file is executable, otherwise <see langword="false"/>.</returns>
	public bool IsExecutable(string file) => File.Exists(file) switch {
		false => false,
		true => OperatingSystem.IsWindows() ? Extensions.Contains(Path.GetExtension(file).ToLowerInvariant()) : TestFileAccess(file, 1 /* X_OK */) == 0
	};

	/// <summary>
	/// Finds the instances of an executable in the specified directory.
	/// </summary>
	/// <param name="directory">The directory path.</param>
	/// <param name="command">The command to be resolved.</param>
	/// <returns>The paths of the executables found.</returns>
	private IEnumerable<string> FindExecutables(string directory, string command) => new string[] { "" }
		.Concat(OperatingSystem.IsWindows() ? Extensions : [])
		.Select(extension => Path.GetFullPath(Path.Join(directory, $"{command}{extension}")))
		.Where(IsExecutable);

	/// <summary>
	/// Tests the accessibility of a file.
	/// </summary>
	/// <param name="path">The path of the file to test.</param>
	/// <param name="mode">The accessibility checks to be performed.</param>
	/// <returns><c>0</c> on success, otherwise <c>-1</c>.</returns>
	[LibraryImport("libc", EntryPoint = "access", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
	private static partial int TestFileAccess(string path, int mode);
}
