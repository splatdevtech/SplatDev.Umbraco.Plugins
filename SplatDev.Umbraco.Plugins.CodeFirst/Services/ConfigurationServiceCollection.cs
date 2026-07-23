namespace SplatDev.Umbraco.Plugins.CodeFirst.Services
{
    using NPoco;

    using System.Collections.Generic;
    public static class ConfigurationServiceCollection
    {
        public static Dictionary<string, ConfigurationService> Collection { get; private set; }
        public static Database Database { get; set; }
        public static string UmbracoRootDirectory { get; set; }

        public static void AddToCollection(string assemblyName, ConfigurationService service)
        {
            Collection = Collection ?? new Dictionary<string, ConfigurationService>();
            if (!Collection.ContainsKey(assemblyName))
                Collection.Add(assemblyName, service);
        }
    }
}
