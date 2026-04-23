namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    using SplatDev.Umbraco.Plugins.CodeFirst.Enums;
    using SplatDev.Umbraco.Plugins.CodeFirst.Interfaces;
    using SplatDev.Umbraco.Plugins.CodeFirst.Models;

    /// <summary>
    /// Extracts and embedded resource and copies it to the specified location
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/13031778/how-can-i-extract-a-file-from-an-embedded-resource-and-save-it-to-disk"/>
    public static class AssetHelpers
    {
        // NOTE: FileStreamResult (System.Web.Mvc) is not available in .NET 8/10.
        // TODO: Return a Stream or use Microsoft.AspNetCore.Mvc.FileStreamResult instead.
        public static Stream? GetAsset(string assemblyName, string id)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            var resourceName = assembly.GetManifestResourceNames().ToList().Find(f => f.EndsWith(id));
            if (resourceName == null) return null;
            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Adds the type of the allowed document.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="parentAlias">The parent alias.</param>
        /// <param name="childAlias">The child alias.</param>
        /// <param name="order">The order.</param>
        public static void AddAllowedDocumentType(IContentTypeService service, string parentAlias, string childAlias, int order = -1)
        {
            var parent = service.Get(parentAlias);
            var child = service.Get(childAlias);
            var allowed = parent!.AllowedContentTypes!.ToList();
            allowed.Add(new ContentTypeSort(child!.Id, order == -1 ? allowed.Count() : order));
            parent.AllowedContentTypes = allowed;
            service.Save(parent);
        }

        /// <summary>
        /// Adds the template.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="service">The service.</param>
        /// <param name="template">The template.</param>
        public static void AddTemplate(this IContentType type, IContentTypeService service, Template template)
        {
            var templates = type.AllowedTemplates!.ToList();
            templates.Add(template);
            type.AllowedTemplates = templates;
            service.Save(type);
        }

        /// <summary>
        /// Assets the already exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="outputDirectory">The output directory.</param>
        /// <param name="appRoot">The application root (ContentRootPath in .NET 8+)</param>
        /// <returns></returns>
        public static bool AssetAlreadyExists(string fileName, string outputDirectory, string appRoot)
        {
            return CheckExists(appRoot, fileName, outputDirectory);
        }

        /// <summary>
        /// Checks the exists.
        /// </summary>
        /// <param name="appRoot">The application root.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="outputDirectory">The output directory.</param>
        /// <returns></returns>
        public static bool CheckExists(string appRoot, string fileName, string outputDirectory)
        {
            var fileFullPath = Path.Combine(appRoot, outputDirectory, fileName);
            return System.IO.File.Exists(fileFullPath);
        }

        /// <summary>
        /// Copies the physical assets.
        /// </summary>
        /// <param name="assets">The embedded resource.</param>
        public static void CopyPhysicalAssets<T>(T type) where T : Type, IAssets
        {
            var instance = (IAssets)type.GetInstance();
            var list = GetListOfAssets(instance, AssetTypes.All);
            var assembly = Assembly.GetExecutingAssembly();

            if (list.Any())
            {
                //Copy physical files to destination
                assembly.ExtractAsset(list);
            }
        }

        /// <summary>
        /// Extracts the embedded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="files">The files.</param>
        public static void ExtractAsset(this Assembly assembly, IEnumerable<Asset> files)
        {
            // NOTE: In .NET 8+, HostingEnvironment.MapPath is not available.
            // TODO: Inject IWebHostEnvironment and use env.ContentRootPath or env.WebRootPath.
            var appRoot = AppContext.BaseDirectory;
            var assemblyName = assembly.GetName().Name;

            foreach (var file in files)
            {
                if (file.AssetType == AssetTypes.Directory)
                {
                    foreach (var rec in assembly.GetManifestResourceNames().ToList().Where(x => x.Contains(file.ResourceLocation)))
                        Copy(assembly, rec, file.OutputDirectory, appRoot, assemblyName!, file.ResourceLocation, true, file.Replace, file.AddToVisualStudioProject, file.DependentUpon, file.DependentUponFile);
                }
                else if (file.AssetType == AssetTypes.CreateDirectoryOnly)
                {
                    CreateDirectory(file.OutputDirectory);
                }
                else
                {
                    Copy(assembly, file.FileName, file.OutputDirectory, appRoot, assemblyName!, file.ResourceLocation, false, file.Replace, file.AddToVisualStudioProject, file.DependentUpon, file.DependentUponFile, createBackup: file.CreateBackup);
                }
            }
        }

        /// <summary>
        /// Gets all files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFiles(string path, string extension)
        {
            return Directory.EnumerateFiles(path, extension, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Gets all files containing.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetAllFilesContaining(string path, string pattern, string extension = "*.*")
        {
            var files = GetAllFiles(path, extension);
            var matches = new Dictionary<string, List<string>>();
            foreach (var file in files)
            {
                var fileMatches = HasMatch(file, pattern);
                if (fileMatches.Count > 0)
                {
                    matches.Add(file, fileMatches);
                }
            }
            return matches;
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="resourceLocation">The resource location.</param>
        /// <returns></returns>
        public static Stream? GetStream(Assembly assembly, string filename, string resourceLocation)
        {
            string path = $"{assembly.GetName().Name}.{resourceLocation}.{filename}";
            return assembly.GetManifestResourceStream(path);
        }

        /// <summary>
        /// Sets the template.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <param name="templateAlias">The template alias.</param>
        public static void SetTemplate(IContentService service, IFileService fileService, int nodeId, string templateAlias)
        {
            var node = service.GetById(nodeId);
            var template = fileService.GetTemplate(templateAlias);
            node!.TemplateId = template!.Id;
            service.Save(node);
        }

        // NOTE: AddToVisualStudioProject used Microsoft.Build and System.Web.HttpContext - removed in .NET 8.
        // TODO: This functionality is not applicable for SDK-style projects; remove or reimplement if needed.

        /// <summary>
        /// Copies the specified assembly.
        /// </summary>
        private static void Copy(Assembly assembly, string fileName, string outputDirectory, string appRoot, string assemblyName, string resourceLocation, bool isDirectory, bool? replace = false, bool? addtoVSProj = false, bool? dependentUpon = false, string dependentUponFile = "", bool createBackup = false, Microsoft.Extensions.Logging.ILogger? logger = null)
        {
            string fileFullPath = string.Empty;
            string path;
            if (isDirectory)
            {
                path = fileName;
                fileName = fileName.Replace(assemblyName + ".", "").Replace(resourceLocation + ".", "");
            }
            else
            {
                path = $"{assemblyName}.{resourceLocation}.{fileName}";
            }

            try
            {
                using (Stream? stream = assembly.GetManifestResourceStream(path))
                {
                    if (stream == null) return;

                    var outputPath = Path.Combine(appRoot, outputDirectory);
                    CreateDirectory(outputPath);

                    fileFullPath = Path.Combine(appRoot, outputDirectory, fileName);
                    if (createBackup)
                    {
                        if (System.IO.File.Exists(fileFullPath) && new FileInfo(fileFullPath).Length != stream.Length)
                            new FileInfo(fileFullPath).MoveTo($"{fileFullPath}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");
                    }

                    var fileMode = replace!.Value ? FileMode.OpenOrCreate : FileMode.Create;
                    if (!System.IO.File.Exists(fileFullPath) || replace.Value)
                    {
                        using (FileStream fileStream = new FileStream(fileFullPath, fileMode))
                        {
                            for (int i = 0; i < stream.Length; i++)
                            {
                                fileStream.WriteByte((byte)stream.ReadByte());
                            }
                        }
                        // NOTE: AddToVisualStudioProject no longer applicable for SDK-style projects
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogError(ex, "Copy failed. Path:{Path}; File: {FileName}; Target: {Target}", path, fileName, fileFullPath);
                }
            }
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private static IEnumerable<Asset> GetListOfAssets(IAssets assets, AssetTypes assetType)
        {
            List<Asset> list = new List<Asset>();

            foreach (var resource in assets.Assets)
            {
                if (resource.AssetType == assetType || assetType == AssetTypes.All)
                    list.Add(resource);
            }
            return list;
        }

        /// <summary>
        /// Determines whether the specified path has match.
        /// </summary>
        private static List<string> HasMatch(string path, string matchText)
        {
            Regex regex = new Regex(matchText);
            var matches = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        string v = match.Groups[1].Value;
                        matches.Add(v);
                    }
                }
            }
            return matches;
        }
    }
}
