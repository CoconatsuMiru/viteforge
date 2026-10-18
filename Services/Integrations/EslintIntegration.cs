using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class EslintIntegration : IIntegration
{
    private const string EslintPackages =
        "eslint@9 @eslint/js@9 typescript-eslint eslint-plugin-react-hooks eslint-plugin-react-refresh globals";

    private const string EslintConfig = """
        import js from '@eslint/js'
        import globals from 'globals'
        import reactHooks from 'eslint-plugin-react-hooks'
        import reactRefresh from 'eslint-plugin-react-refresh'
        import tseslint from 'typescript-eslint'
        import { defineConfig, globalIgnores } from 'eslint/config'

        export default defineConfig([
          globalIgnores(['dist']),
          {
            files: ['**/*.{ts,tsx}'],
            extends: [
              js.configs.recommended,
              tseslint.configs.recommended,
              reactHooks.configs.flat.recommended,
              reactRefresh.configs.vite,
            ],
            languageOptions: {
              ecmaVersion: 2020,
              globals: globals.browser,
            },
          },
        ])
        """;

    public string Name => "ESLint";

    // Always runs: the Vite template ships with Oxlint, which is replaced by ESLint or removed.
    public bool IsSelected(ProjectOptions options) => true;

    public bool Apply(ProjectOptions options)
    {
        if (!RemoveOxlint(options))
        {
            return false;
        }

        if (!options.UseEslint)
        {
            ConsoleUI.Info("Linting removed from the project.");
            return true;
        }

        if (!PackageInstaller.Add(options, EslintPackages, dev: true))
        {
            ConsoleUI.Error("Could not install ESLint.");
            return false;
        }

        var configPath = Path.Combine(options.ProjectPath, "eslint.config.js");
        File.WriteAllText(configPath, EslintConfig + "\n");

        PackageJsonEditor.SetScript(options.ProjectPath, "lint", "eslint .");

        return true;
    }

    private static bool RemoveOxlint(ProjectOptions options)
    {
        var packages = PackageJsonEditor.GetDevDependencies(options.ProjectPath)
            .Where(name => name.StartsWith("oxlint"))
            .ToList();

        if (packages.Count > 0 && !PackageInstaller.Remove(options, string.Join(" ", packages)))
        {
            ConsoleUI.Error("Could not remove Oxlint.");
            return false;
        }

        var oxlintConfig = Path.Combine(options.ProjectPath, ".oxlintrc.json");

        if (File.Exists(oxlintConfig))
        {
            File.Delete(oxlintConfig);
        }

        PackageJsonEditor.RemoveScript(options.ProjectPath, "lint");

        return true;
    }
}