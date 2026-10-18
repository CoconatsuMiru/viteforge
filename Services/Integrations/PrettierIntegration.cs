using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class PrettierIntegration : IIntegration
{
    private const string Config = """
        {
          "semi": false,
          "singleQuote": true,
          "trailingComma": "all",
          "printWidth": 100
        }
        """;

    private const string IgnoreFile = """
        dist
        node_modules
        package-lock.json
        pnpm-lock.yaml
        yarn.lock
        """;

    public string Name => "Prettier";

    public bool IsSelected(ProjectOptions options) => options.UsePrettier;

    public bool Apply(ProjectOptions options)
    {
        if (!PackageInstaller.Add(options, "prettier", dev: true))
        {
            ConsoleUI.Error("Could not install Prettier.");
            return false;
        }

        File.WriteAllText(Path.Combine(options.ProjectPath, ".prettierrc"), Config + "\n");
        File.WriteAllText(Path.Combine(options.ProjectPath, ".prettierignore"), IgnoreFile + "\n");

        PackageJsonEditor.SetScript(options.ProjectPath, "format", "prettier --write .");

        ConsoleUI.Step("Formatting project files...");
        var manager = options.PackageManager.ToCommand();

        if (CommandRunner.RunLive($"{manager} run format", options.ProjectPath) != 0)
        {
            ConsoleUI.Error("Formatting failed.");
            return false;
        }

        return true;
    }
}