namespace SplatDev.Umbraco.Tools.T4.Plugins.Templates;

public static class PackageManifestV13Template
{
    public static string Render(string name) => $$"""
{
  "name": "{{name}}",
  "version": "1.0.0",
  "allowPackageTelemetry": true,
  "javascript": [
    "/App_Plugins/{{name}}/{{name}}.controller.js"
  ],
  "css": [
    "/App_Plugins/{{name}}/{{name}}.css"
  ]
}
""";
}
