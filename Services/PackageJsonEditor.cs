using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ReactGen.Services;

public static class PackageJsonEditor
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        // Keeps characters like && readable instead of escaping them.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void SetScript(string projectPath, string name, string command)
    {
        var path = GetPath(projectPath);
        var root = Load(path);

        if (root["scripts"] is not JsonObject scripts)
        {
            scripts = new JsonObject();
            root["scripts"] = scripts;
        }

        scripts[name] = command;
        Save(path, root);
    }

    public static void RemoveScript(string projectPath, string name)
    {
        var path = GetPath(projectPath);
        var root = Load(path);

        if (root["scripts"] is JsonObject scripts && scripts.Remove(name))
        {
            Save(path, root);
        }
    }

    public static List<string> GetDevDependencies(string projectPath)
    {
        var root = Load(GetPath(projectPath));

        return root["devDependencies"] is JsonObject dependencies
            ? dependencies.Select(item => item.Key).ToList()
            : new List<string>();
    }

    private static string GetPath(string projectPath) => Path.Combine(projectPath, "package.json");

    private static JsonObject Load(string path) =>
        JsonNode.Parse(File.ReadAllText(path))!.AsObject();

    private static void Save(string path, JsonObject root) =>
        File.WriteAllText(path, root.ToJsonString(WriteOptions) + "\n");
}