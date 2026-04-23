namespace UmbracoCms.Tools.Packager;

public class AssetBundler
{
    private readonly string _sourceDir;
    private readonly string _outputDir;

    public AssetBundler(string sourceDir, string outputDir)
    {
        _sourceDir = sourceDir;
        _outputDir = outputDir;
    }

    public void Bundle()
    {
        var appPluginsSource = Path.Combine(_sourceDir, "App_Plugins");
        if (!Directory.Exists(appPluginsSource))
        {
            Console.WriteLine("No App_Plugins directory found — skipping asset bundle.");
            return;
        }

        var appPluginsDest = Path.Combine(_outputDir, "App_Plugins");
        CopyDirectory(appPluginsSource, appPluginsDest);
        Console.WriteLine($"Bundled App_Plugins to: {appPluginsDest}");
    }

    private static void CopyDirectory(string source, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(source))
        {
            var destFile = Path.Combine(dest, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }
        foreach (var dir in Directory.GetDirectories(source))
        {
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
        }
    }
}
