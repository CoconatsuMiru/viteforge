using ReactGen.Services;
using ReactGen.Services.Integrations;
using ReactGen.UI;

ConsoleUI.Title("ReactGen - React + TypeScript + Vite project generator");

var options = OptionsPrompt.Ask();

Console.WriteLine();

if (!ToolChecker.EnsureAvailable(options.PackageManager))
{
    ConsoleUI.Error("Fix the problems above and try again.");
    return;
}

SummaryPrinter.Show(options);

if (!ConsoleUI.AskYesNo("Create this project?", defaultValue: true))
{
    ConsoleUI.Warning("Cancelled. Nothing was created.");
    return;
}

if (!ProjectScaffolder.Create(options))
{
    ConsoleUI.Error("Project creation failed. See the messages above.");
    return;
}

if (!IntegrationRunner.Run(options))
{
    ConsoleUI.Error("Integration setup failed. See the messages above.");
    return;
}

Console.WriteLine();
ConsoleUI.Success($"Project created at {options.ProjectPath}");