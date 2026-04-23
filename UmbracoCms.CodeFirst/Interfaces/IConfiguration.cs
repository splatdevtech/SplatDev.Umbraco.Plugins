namespace UmbracoCms.CodeFirst.Interfaces
{
    public interface IConfiguration
    {
        string PluginName { get; }
        string AssemblyName { get; }
        string PackageRootDocumentTypeAlias { get; }
    }
}
