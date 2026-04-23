using UmbracoCms.Tools.T4.Plugins.Templates;

namespace UmbracoCms.Tools.T4.Plugins;

public class PluginScaffolder
{
    private readonly string _name;
    private readonly string _namespace;
    private readonly string _outputDir;

    public PluginScaffolder(string name, string @namespace, string outputDir)
    {
        _name = name;
        _namespace = @namespace;
        _outputDir = outputDir;
    }

    public void Scaffold()
    {
        Directory.CreateDirectory(_outputDir);

        WriteFile(_outputDir, $"{_name}Composer.cs", ComposerTemplate.Render(_name, _namespace));
        WriteFile(_outputDir, $"{_name}Controller.cs", ControllerTemplate.Render(_name, _namespace));
        WriteFile(_outputDir, $"I{_name}Service.cs", ServiceTemplate.RenderInterface(_name, _namespace));
        WriteFile(_outputDir, $"{_name}Service.cs", ServiceTemplate.RenderImplementation(_name, _namespace));

        var pluginsDir = Path.Combine(_outputDir, "App_Plugins", _name);
        Directory.CreateDirectory(pluginsDir);
        WriteFile(pluginsDir, "umbraco-package.json", PackageManifestTemplate.Render(_name));

        var v13Dir = Path.Combine(pluginsDir, "v13");
        Directory.CreateDirectory(v13Dir);
        WriteFile(v13Dir, "package.manifest", PackageManifestV13Template.Render(_name));
    }

    private static void WriteFile(string dir, string filename, string content)
    {
        var path = Path.Combine(dir, filename);
        File.WriteAllText(path, content);
        Console.WriteLine($"  Created: {path}");
    }
}
