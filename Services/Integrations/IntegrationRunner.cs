using ReactGen.Models;
using ReactGen.UI;

namespace ReactGen.Services.Integrations;

public static class IntegrationRunner
{
    // Add new integrations to this list as they are built.
    // Prettier stays last so it formats files written by the others.
    private static readonly IIntegration[] Integrations =
    {
        new TailwindIntegration(),
        new ReactRouterIntegration(),
        new SupabaseIntegration(),
        new EslintIntegration(),
        new PrettierIntegration()
    };

    // Applies every selected integration. Stops at the first failure.
    public static bool Run(ProjectOptions options)
    {
        foreach (var integration in Integrations)
        {
            if (!integration.IsSelected(options))
            {
                continue;
            }

            Console.WriteLine();
            ConsoleUI.Step($"Configuring {integration.Name}...");

            if (!integration.Apply(options))
            {
                ConsoleUI.Error($"{integration.Name} setup failed.");
                return false;
            }

            ConsoleUI.Success($"{integration.Name} configured.");
        }

        return true;
    }
}