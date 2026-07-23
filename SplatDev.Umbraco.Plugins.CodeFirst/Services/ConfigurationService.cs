namespace SplatDev.Umbraco.Plugins.CodeFirst.Services
{
    using System.Collections.Generic;
    using System.Reflection;
    public class ConfigurationService
    {
        public string PluginName { get; set; }
        public string AssemblyName { get; set; }
        public string PackageRootDocumentTypeAlias { get; set; }
        public string RunningApplicationRootDirectory { get; set; }
        public List<Assembly> Dependencies { get; set; }
        public bool AlreadyInstalled { get; set; }
    }
}
