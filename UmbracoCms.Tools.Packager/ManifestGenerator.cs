using System.Text.Json;

namespace UmbracoCms.Tools.Packager;

public class ManifestGenerator
{
    private readonly string _name;
    private readonly string _version;

    public ManifestGenerator(string name, string version)
    {
        _name = name;
        _version = version;
    }

    public string Generate(string outputDir)
    {
        var pluginsDir = Path.Combine(outputDir, "App_Plugins", _name);
        Directory.CreateDirectory(pluginsDir);

        var manifest = new
        {
            name = _name,
            version = _version,
            allowPackageTelemetry = true,
            extensions = Array.Empty<object>(),
        };

        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
        var manifestPath = Path.Combine(pluginsDir, "umbraco-package.json");
        File.WriteAllText(manifestPath, json);
        Console.WriteLine($"Generated manifest: {manifestPath}");
        return manifestPath;
    }
}
