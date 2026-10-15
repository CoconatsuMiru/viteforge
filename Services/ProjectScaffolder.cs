using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services;

public static class ProjectScaffolder
{
    // Returns true if the project was created and dependencies installed.
    public static bool Create(ProjectOptions options)
    {
        ConsoleUI.Title("Creating project");

        try
        {
            Directory.CreateDirectory(options.DestinationFolder);
        }
        catch (Exception ex)
        {
            ConsoleUI.Error($"Could not create the destination folder: {ex.Message}");
            return false;
        }

        ConsoleUI.Step("Scaffolding React + TypeScript + Vite...");
        var createExitCode = CommandRunner.RunLive(
            GetCreateCommand(options),
            options.DestinationFolder);

        if (createExitCode != 0)
        {
            ConsoleUI.Error($"Scaffolding failed (exit code {createExitCode}).");
            return false;
        }

        ConsoleUI.Success("Project files created.");
        Console.WriteLine();

        var manager = options.PackageManager.ToCommand();
        ConsoleUI.Step($"Installing dependencies with {manager}...");
        var installExitCode = CommandRunner.RunLive($"{manager} install", options.ProjectPath);

        if (installExitCode != 0)
        {
            ConsoleUI.Error($"Dependency install failed (exit code {installExitCode}).");
            return false;
        }

        ConsoleUI.Success("Dependencies installed.");
        return true;
    }

    private static string GetCreateCommand(ProjectOptions options)
    {
        const string flags = "--template react-ts --no-interactive";

        return options.PackageManager switch
        {
            // npm needs "--" before the flags meant for create-vite.
            PackageManager.Npm => $"npm create vite@latest {options.ProjectName} -- {flags}",
            PackageManager.Pnpm => $"pnpm create vite {options.ProjectName} {flags}",
            PackageManager.Yarn => $"yarn create vite {options.ProjectName} {flags}",
            _ => throw new ArgumentOutOfRangeException(nameof(options))
        };
    }
}