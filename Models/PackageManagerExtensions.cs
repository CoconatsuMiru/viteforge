namespace ReactGen.Models;

public static class PackageManagerExtensions
{
    // The command name typed in the terminal.
    public static string ToCommand(this PackageManager manager) => manager switch
    {
        PackageManager.Npm => "npm",
        PackageManager.Pnpm => "pnpm",
        PackageManager.Yarn => "yarn",
        _ => throw new ArgumentOutOfRangeException(nameof(manager))
    };

    // The command that starts the Vite dev server.
    public static string ToDevCommand(this PackageManager manager) => manager switch
    {
        PackageManager.Npm => "npm run dev",
        PackageManager.Pnpm => "pnpm dev",
        PackageManager.Yarn => "yarn dev",
        _ => throw new ArgumentOutOfRangeException(nameof(manager))
    };
}