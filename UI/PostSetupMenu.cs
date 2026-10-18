using ReactGen.Models;
using ReactGen.Services;

namespace ReactGen.UI;

public static class PostSetupMenu
{
    private static readonly string[] Choices =
    {
        "Open in VS Code",
        "Show commands to start the dev server",
        "Exit"
    };

    public static void Show(ProjectOptions options)
    {
        while (true)
        {
            Console.WriteLine();
            var choice = ConsoleUI.AskChoice("What next?", Choices, defaultIndex: 2);

            switch (choice)
            {
                case 0:
                    OpenInVsCode(options);
                    break;
                case 1:
                    ShowCommands(options);
                    break;
                default:
                    return;
            }
        }
    }

    private static void OpenInVsCode(ProjectOptions options)
    {
        var check = CommandRunner.RunCapture("code", "--version");

        if (!check.Success)
        {
            ConsoleUI.Error("The 'code' command was not found.");
            ConsoleUI.Info("In VS Code, open the Command Palette and run: Shell Command: Install 'code' command in PATH");
            return;
        }

        var result = CommandRunner.RunCapture("code", $"\"{options.ProjectPath}\"");

        if (result.Success)
        {
            ConsoleUI.Success("Opened in VS Code.");
        }
        else
        {
            ConsoleUI.Error("Could not open VS Code.");
        }
    }

    private static void ShowCommands(ProjectOptions options)
    {
        Console.WriteLine();
        ConsoleUI.Info("Run these commands:");
        ConsoleUI.Info($"  cd \"{options.ProjectPath}\"");
        ConsoleUI.Info($"  {options.PackageManager.ToDevCommand()}");

        if (options.UseSupabase)
        {
            Console.WriteLine();
            ConsoleUI.Warning("Fill in your Supabase URL and key in .env before using Supabase.");
        }
    }
}