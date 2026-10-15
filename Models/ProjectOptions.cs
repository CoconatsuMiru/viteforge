namespace ReactGen.Models;

public class ProjectOptions
{
    public string ProjectName { get; set; } = "";
    public string DestinationFolder { get; set; } = "";
    public PackageManager PackageManager { get; set; } = PackageManager.Npm;

    public bool UseTailwind { get; set; }
    public bool UseEslint { get; set; }
    public bool UsePrettier { get; set; }
    public bool UseReactRouter { get; set; }
    public bool UseSupabase { get; set; }

    public string ProjectPath => Path.Combine(DestinationFolder, ProjectName);
}