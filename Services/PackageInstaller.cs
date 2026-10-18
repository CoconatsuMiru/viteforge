using ReactGen.Models;

namespace ReactGen.Services;

public static class PackageInstaller
{
    // Adds packages to the generated project using the selected package manager.
    // Returns true on success.
    public static bool Add(ProjectOptions options, string packages, bool dev = false)
    {
        var verb = options.PackageManager == PackageManager.Npm ? "install" : "add";
        var devFlag = dev ? " -D" : "";
        var command = $"{options.PackageManager.ToCommand()} {verb}{devFlag} {packages}";

        return CommandRunner.RunLive(command, options.ProjectPath) == 0;
    }
}