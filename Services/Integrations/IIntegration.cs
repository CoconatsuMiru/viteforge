using ReactGen.Models;

namespace ReactGen.Services.Integrations;

public interface IIntegration
{
    string Name { get; }

    // True if the user selected this integration.
    bool IsSelected(ProjectOptions options);

    // Installs and configures the integration. Returns true on success.
    bool Apply(ProjectOptions options);
}