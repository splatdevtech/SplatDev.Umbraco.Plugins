using UmbracoCms.Tools.T4.Themes.Templates;

namespace UmbracoCms.Tools.T4.Themes;

public class ThemeScaffolder
{
    private readonly string _name;
    private readonly string _outputDir;

    public ThemeScaffolder(string name, string outputDir)
    {
        _name = name;
        _outputDir = outputDir;
    }

    public void Scaffold()
    {
        var viewsShared = Path.Combine(_outputDir, "Views", "Shared");
        var viewsHome = Path.Combine(_outputDir, "Views", "Home");
        var cssDir = Path.Combine(_outputDir, "wwwroot", "css");
        var pluginsDir = Path.Combine(_outputDir, "App_Plugins", _name);

        Directory.CreateDirectory(viewsShared);
        Directory.CreateDirectory(viewsHome);
        Directory.CreateDirectory(cssDir);
        Directory.CreateDirectory(pluginsDir);

        WriteFile(viewsShared, "_Layout.cshtml", LayoutViewTemplate.Render(_name));
        WriteFile(viewsHome, "Index.cshtml", HomeViewTemplate.Render(_name));
        WriteFile(cssDir, $"{_name.ToLowerInvariant()}.css", CssTemplate.Render(_name));
        WriteFile(_outputDir, "theme.json", ThemeJsonTemplate.Render(_name));
        WriteFile(pluginsDir, "umbraco-package.json", PackageManifestTemplate.Render(_name));
    }

    private static void WriteFile(string dir, string filename, string content)
    {
        var path = Path.Combine(dir, filename);
        File.WriteAllText(path, content);
        Console.WriteLine($"  Created: {path}");
    }
}
