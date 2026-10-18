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

        return Run(options, $"{verb}{devFlag} {packages}");
    }

    // Removes packages from the generated project. Returns true on success.
    public static bool Remove(ProjectOptions options, string packages)
    {
        var verb = options.PackageManager == PackageManager.Npm ? "uninstall" : "remove";

        return Run(options, $"{verb} {packages}");
    }

    private static bool Run(ProjectOptions options, string arguments)
    {
        var command = $"{options.PackageManager.ToCommand()} {arguments}";

        return CommandRunner.RunLive(command, options.ProjectPath) == 0;
    }
}