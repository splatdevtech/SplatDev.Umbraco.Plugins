namespace SplatDev.Umbraco.Tools.T4.Plugins.Templates;

public static class ServiceTemplate
{
    public static string RenderInterface(string name, string @namespace) => $$"""
namespace {{@namespace}};

public interface I{{name}}Service
{
    IEnumerable<object> GetAll();
}
""";

    public static string RenderImplementation(string name, string @namespace) => $$"""
namespace {{@namespace}};

public class {{name}}Service : I{{name}}Service
{
    public IEnumerable<object> GetAll() => Enumerable.Empty<object>();
}
""";
}
