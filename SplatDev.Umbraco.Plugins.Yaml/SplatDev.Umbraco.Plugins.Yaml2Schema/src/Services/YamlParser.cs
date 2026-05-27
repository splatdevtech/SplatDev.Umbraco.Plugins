using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class YamlParser
    {
        public YamlRoot ParseYaml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"YAML file not found: {filePath}");
            }

            try
            {
                var fileContents = File.ReadAllText(filePath);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

                var umbraco = deserializer.Deserialize<UmbracoConfig>(fileContents);

                return new YamlRoot { Umbraco = umbraco ?? new UmbracoConfig() };
            }
            catch (YamlException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to parse YAML file '{filePath}': {ex.Message}",
                    ex
                );
            }
        }
    }
}
