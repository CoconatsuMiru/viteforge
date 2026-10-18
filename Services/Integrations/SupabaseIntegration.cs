using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public class SupabaseIntegration : IIntegration
{
    private const string ClientTs = """
        import { createClient } from '@supabase/supabase-js'

        const supabaseUrl = import.meta.env.VITE_SUPABASE_URL
        const supabaseKey = import.meta.env.VITE_SUPABASE_PUBLISHABLE_KEY

        if (!supabaseUrl || !supabaseKey) {
          throw new Error(
            'Missing Supabase settings. Fill in VITE_SUPABASE_URL and VITE_SUPABASE_PUBLISHABLE_KEY in .env',
          )
        }

        export const supabase = createClient(supabaseUrl, supabaseKey)
        """;

    private const string EnvContent = """
        VITE_SUPABASE_URL=
        VITE_SUPABASE_PUBLISHABLE_KEY=
        """;

    public string Name => "Supabase";

    public bool IsSelected(ProjectOptions options) => options.UseSupabase;

    public bool Apply(ProjectOptions options)
    {
        if (!PackageInstaller.Add(options, "@supabase/supabase-js"))
        {
            ConsoleUI.Error("Could not install the Supabase client.");
            return false;
        }

        var root = options.ProjectPath;

        var libFolder = Path.Combine(root, "src", "lib");
        Directory.CreateDirectory(libFolder);
        File.WriteAllText(Path.Combine(libFolder, "supabase.ts"), ClientTs + "\n");

        File.WriteAllText(Path.Combine(root, ".env.example"), EnvContent + "\n");

        var envPath = Path.Combine(root, ".env");

        if (!File.Exists(envPath))
        {
            File.WriteAllText(envPath, EnvContent + "\n");
        }

        IgnoreEnvFile(root);

        ConsoleUI.Info("Fill in your keys in .env (Supabase dashboard > Project Settings > API).");
        return true;
    }

    private static void IgnoreEnvFile(string projectPath)
    {
        var gitignorePath = Path.Combine(projectPath, ".gitignore");

        var lines = File.Exists(gitignorePath)
            ? File.ReadAllLines(gitignorePath)
            : Array.Empty<string>();

        if (lines.Any(line => line.Trim() == ".env"))
        {
            return;
        }

        File.AppendAllText(
            gitignorePath,
            $"{Environment.NewLine}# Environment variables{Environment.NewLine}.env{Environment.NewLine}");
    }
}