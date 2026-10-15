using System.Text.RegularExpressions;

namespace ReactGen.Services;

// Every Validate method returns an error message, or null if the input is valid.
public static class InputValidator
{
    private const int MaxNameLength = 50;

    // Lowercase letters and digits, optionally separated by single "-" or "_".
    private static readonly Regex NamePattern =
        new("^[a-z0-9]+([-_][a-z0-9]+)*$", RegexOptions.Compiled);

    private static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "node_modules", "react", "react-dom", "vite",
        "con", "prn", "aux", "nul",
        "com1", "com2", "com3", "com4",
        "lpt1", "lpt2", "lpt3"
    };

    // Removes surrounding spaces and quotes (Windows adds quotes with "Copy as path").
    public static string NormalizePath(string path) => path.Trim().Trim('"');

    public static string? ValidateProjectName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Name cannot be empty.";
        }

        if (name.Length > MaxNameLength)
        {
            return $"Name must be {MaxNameLength} characters or fewer.";
        }

        if (!NamePattern.IsMatch(name))
        {
            return "Use only lowercase letters, numbers, '-' and '_' (example: my-app).";
        }

        if (ReservedNames.Contains(name))
        {
            return $"'{name}' is a reserved name. Choose another.";
        }

        return null;
    }

    public static string? ValidateDestinationFolder(string input)
    {
        var path = NormalizePath(input);

        if (string.IsNullOrEmpty(path))
        {
            return "Folder cannot be empty.";
        }

        if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            return "Path contains invalid characters.";
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(path);
        }
        catch (Exception)
        {
            return "Path is not valid.";
        }

        if (!Path.IsPathRooted(path))
        {
            return "Enter a full path (example: C:\\Dev).";
        }

        if (File.Exists(fullPath))
        {
            return "That path is a file, not a folder.";
        }

        var root = Path.GetPathRoot(fullPath);
        if (root is null || !Directory.Exists(root))
        {
            return "That drive does not exist.";
        }

        // A folder that doesn't exist yet is fine; it will be created later.
        return null;
    }

    public static string? ValidateProjectFolder(string destinationFolder, string projectName)
    {
        var fullPath = Path.Combine(NormalizePath(destinationFolder), projectName);

        if (File.Exists(fullPath))
        {
            return $"A file named '{projectName}' already exists there.";
        }

        if (Directory.Exists(fullPath) && Directory.EnumerateFileSystemEntries(fullPath).Any())
        {
            return $"Folder '{fullPath}' already exists and is not empty.";
        }

        return null;
    }
}