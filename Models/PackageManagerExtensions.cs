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
}