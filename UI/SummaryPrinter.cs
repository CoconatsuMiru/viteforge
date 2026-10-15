using ReactGen.Models;

namespace ReactGen.UI;

public static class SummaryPrinter
{
    public static void Show(ProjectOptions options)
    {
        ConsoleUI.Title("Summary");

        ConsoleUI.Info($"Project name:     {options.ProjectName}");
        ConsoleUI.Info($"Location:         {options.ProjectPath}");
        ConsoleUI.Info($"Package manager:  {options.PackageManager.ToCommand()}");
        ConsoleUI.Info("Template:         React + TypeScript + Vite");
        ConsoleUI.Info($"Tailwind CSS:     {YesNo(options.UseTailwind)}");
        ConsoleUI.Info($"ESLint:           {YesNo(options.UseEslint)}");
        ConsoleUI.Info($"Prettier:         {YesNo(options.UsePrettier)}");
        ConsoleUI.Info($"React Router:     {YesNo(options.UseReactRouter)}");
        ConsoleUI.Info($"Supabase:         {YesNo(options.UseSupabase)}");
        Console.WriteLine();
    }

    private static string YesNo(bool value) => value ? "Yes" : "No";
}