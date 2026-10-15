using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services;

public static class ToolChecker
{
    // Returns true only if every required tool is available.
    public static bool EnsureAvailable(PackageManager manager)
    {
        ConsoleUI.Step("Checking required tools...");

        var nodeOk = CheckNode();
        var managerOk = CheckPackageManager(manager);

        return nodeOk && managerOk;
    }

    private static bool CheckNode()
    {
        var result = CommandRunner.RunCapture("node", "--version");

        if (!result.Success)
        {
            ConsoleUI.Error("Node.js was not found. Install the LTS version from https://nodejs.org");
            return false;
        }

        var text = result.Output.Trim();

        if (!Version.TryParse(text.TrimStart('v'), out var version))
        {
            ConsoleUI.Warning($"Node.js found, but the version could not be read ({text}). Continuing.");
            return true;
        }

        if (!IsSupportedNode(version))
        {
            ConsoleUI.Error($"Node.js {text} is too old. Vite needs Node 20.19+ or 22.12+. Update from https://nodejs.org");
            return false;
        }

        ConsoleUI.Success($"Node.js {text}");
        return true;
    }

    private static bool IsSupportedNode(Version version) =>
        (version.Major == 20 && version.Minor >= 19) ||
        (version.Major == 22 && version.Minor >= 12) ||
        version.Major >= 23;

    private static bool CheckPackageManager(PackageManager manager)
    {
        var command = manager.ToCommand();
        var result = CommandRunner.RunCapture(command, "--version");

        if (!result.Success)
        {
            ConsoleUI.Error($"{command} was not found.");

            if (manager != PackageManager.Npm)
            {
                ConsoleUI.Info($"Install it with: npm install -g {command}");
            }

            return false;
        }

        ConsoleUI.Success($"{command} {result.Output.Trim()}");
        return true;
    }
}