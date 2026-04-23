namespace UmbracoCms.Tools.T4.Plugins.Templates;

public static class PackageManifestTemplate
{
    public static string Render(string name) => $$"""
{
  "name": "{{name}}",
  "version": "1.0.0",
  "allowPackageTelemetry": true,
  "extensions": [
    {
      "type": "backofficeEntryPoint",
      "alias": "{{name}}.entrypoint",
      "name": "{{name}} Entry Point",
      "js": "/App_Plugins/{{name}}/{{name}}.js"
    }
  ]
}
""";
}
