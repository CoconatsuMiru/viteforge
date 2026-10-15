using ReactGen.Models;
using ReactGen.Services;

namespace ReactGen.UI;

public static class OptionsPrompt
{
    public static ProjectOptions Ask()
    {
        var options = new ProjectOptions();

        options.ProjectName = ConsoleUI.AskText(
            "Project name",
            validate: InputValidator.ValidateProjectName);

        // Repeats until the destination is valid and the project folder is free.
        while (true)
        {
            var destination = ConsoleUI.AskText(
                "Destination folder",
                defaultValue: @"C:\Dev",
                validate: InputValidator.ValidateDestinationFolder);

            options.DestinationFolder = InputValidator.NormalizePath(destination);

            var folderError = InputValidator.ValidateProjectFolder(
                options.DestinationFolder,
                options.ProjectName);

            if (folderError is null)
            {
                break;
            }

            ConsoleUI.Error(folderError);
        }

        var managerIndex = ConsoleUI.AskChoice("Package manager:", new[] { "npm", "pnpm", "yarn" });
        options.PackageManager = (PackageManager)managerIndex;

        Console.WriteLine();
        ConsoleUI.Info("Optional integrations:");
        options.UseTailwind = ConsoleUI.AskYesNo("Tailwind CSS?", defaultValue: true);
        options.UseEslint = ConsoleUI.AskYesNo("ESLint?", defaultValue: true);
        options.UsePrettier = ConsoleUI.AskYesNo("Prettier?", defaultValue: true);
        options.UseReactRouter = ConsoleUI.AskYesNo("React Router?", defaultValue: false);
        options.UseSupabase = ConsoleUI.AskYesNo("Supabase?", defaultValue: false);

        return options;
    }
}