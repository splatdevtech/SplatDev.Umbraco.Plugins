namespace UmbracoCms.Tools.T4.Themes.Templates;

public static class ThemeJsonTemplate
{
    public static string Render(string name) => $$"""
{
  "name": "{{name}}",
  "version": "1.0.0",
  "umbracoTarget": "13+",
  "description": "{{name}} theme for Umbraco",
  "author": "SplatDev",
  "thumbnail": "/App_Plugins/{{name}}/thumbnail.png"
}
""";
}
