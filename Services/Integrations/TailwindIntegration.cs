using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class TailwindIntegration : IIntegration
{
    public string Name => "Tailwind CSS";

    public bool IsSelected(ProjectOptions options) => options.UseTailwind;

    public bool Apply(ProjectOptions options)
    {
        if (!PackageInstaller.Add(options, "tailwindcss @tailwindcss/vite"))
        {
            ConsoleUI.Error("Could not install Tailwind packages.");
            return false;
        }

        if (!AddVitePlugin(options.ProjectPath))
        {
            return false;
        }

        var cssPath = Path.Combine(options.ProjectPath, "src", "index.css");
        File.WriteAllText(cssPath, "@import \"tailwindcss\";\n");

        return true;
    }

    private static bool AddVitePlugin(string projectPath)
    {
        var configPath = Path.Combine(projectPath, "vite.config.ts");

        if (!File.Exists(configPath))
        {
            ConsoleUI.Error("vite.config.ts was not found.");
            return false;
        }

        var text = File.ReadAllText(configPath);

        if (text.Contains("@tailwindcss/vite"))
        {
            return true;
        }

        const string pluginsBefore = "plugins: [react()]";

        if (!text.Contains(pluginsBefore))
        {
            ConsoleUI.Error("Could not update vite.config.ts automatically.");
            ConsoleUI.Info("Add this manually: import tailwindcss from '@tailwindcss/vite'");
            ConsoleUI.Info("and add tailwindcss() to the plugins array.");
            return false;
        }

        text = text.Replace(pluginsBefore, "plugins: [react(), tailwindcss()]");
        text = "import tailwindcss from '@tailwindcss/vite'\n" + text;

        File.WriteAllText(configPath, text);
        return true;
    }
}