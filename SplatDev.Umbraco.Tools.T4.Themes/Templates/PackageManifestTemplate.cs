namespace SplatDev.Umbraco.Tools.T4.Themes.Templates;

public static class PackageManifestTemplate
{
    public static string Render(string name) => $$"""
{
  "name": "{{name}}",
  "version": "1.0.0",
  "allowPackageTelemetry": true,
  "extensions": []
}
""";
}
