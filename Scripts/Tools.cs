using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Contains operations for building a solution.
/// </summary>
public static class Tools {

	/// <summary>
	/// The configuration to use for project generation.
	/// </summary>
	public static string Configuration => IsRelease ? "Release" : "Debug";

	/// <summary>
	/// Value indicating whether the requested configuration is "Release".
	/// </summary>
	public static bool IsRelease { get; } =
		Environment.GetCommandLineArgs().Skip(1).Any(arg => arg.Equals("--release", StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// The path to the solution file.
	/// </summary>
	public static FileInfo Solution { get; } =
		new(Directory.EnumerateFiles(Environment.CurrentDirectory, "*.slnx").Single());

	/// <summary>
	/// The project version number.
	/// </summary>
	public static Version Version { get; }

	/// <summary>
	/// Initializes the class.
	/// </summary>
	static Tools() {
		var defaultValue = new Version(0, 0, 0);
		Version = XDocument.Load(Solution.FullName).Root switch {
			null => defaultValue,
			var element => element.Attribute("ProjectVersion") is XAttribute attribute ? Version.Parse(attribute.Value) : defaultValue
		};
	}

	/// <summary>
	/// Cleans the directory located at the specified path.
	/// </summary>
	/// <param name="path">The path of the file system entry to delete.</param>
	public static void ClearItem(string path) =>
		Directory.EnumerateFileSystemEntries(path)
			.Where(entry => Path.GetFileName(entry) != ".gitkeep")
			.ToList()
			.ForEach(RemoveItem);

	/// <summary>
	/// Deletes the file system entry located at the specified path.
	/// </summary>
	/// <param name="path">The path of the file system entry to delete.</param>
	public static void RemoveItem(string path) {
		if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
		else File.Delete(path);
	}

	/// <summary>
	/// Invokes the specified command.
	/// </summary>
	/// <param name="command">The command to invoke.</param>
	/// <param name="arguments">The command arguments.</param>
	public static void StartProcess(string command, string arguments = "") {
		try {
			using var process = Process.Start(command, arguments);
			process.WaitForExit();
			if (process.ExitCode != 0) throw new OperationCanceledException($"Command failed: {command} {arguments}".TrimEnd());
		}
		catch (Exception ex) {
			WriteError(ex.Message);
		}
	}

	/// <summary>
	/// Prints the specified message to the standard error and terminates the process.
	/// </summary>
	/// <param name="message">The message to print.</message>
	public static void WriteError(string message) {
		Console.Error.WriteLine(message);
		Environment.Exit(1);
	}

	/// <summary>
	/// Prints the specified message to the standard output.
	/// </summary>
	/// <param name="message">The message to print.</message>
	public static void WriteOutput(string message) => Console.Out.WriteLine(message);
}
