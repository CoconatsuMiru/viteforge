using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class EslintIntegration : IIntegration
{
    private static readonly string[] ConfigFiles =
    {
        "eslint.config.js",
        "eslint.config.mjs",
        "eslint.config.ts"
    };

    public string Name => "ESLint";

    // Always runs: the Vite template includes ESLint, so this either keeps it or removes it.
    public bool IsSelected(ProjectOptions options) => true;

    public bool Apply(ProjectOptions options)
    {
        if (options.UseEslint)
        {
            ConsoleUI.Info("ESLint is included in the Vite template. Keeping it.");
            return true;
        }

        var packages = PackageJsonEditor.GetDevDependencies(options.ProjectPath)
            .Where(IsEslintPackage)
            .ToList();

        if (packages.Count > 0 && !PackageInstaller.Remove(options, string.Join(" ", packages)))
        {
            ConsoleUI.Error("Could not remove the ESLint packages.");
            return false;
        }

        foreach (var file in ConfigFiles)
        {
            var path = Path.Combine(options.ProjectPath, file);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        PackageJsonEditor.RemoveScript(options.ProjectPath, "lint");

        ConsoleUI.Info("ESLint removed from the project.");
        return true;
    }

    private static bool IsEslintPackage(string name) =>
        name is "eslint" or "@eslint/js" or "typescript-eslint" or "globals" ||
        name.StartsWith("eslint-plugin-") ||
        name.StartsWith("eslint-config-");
}