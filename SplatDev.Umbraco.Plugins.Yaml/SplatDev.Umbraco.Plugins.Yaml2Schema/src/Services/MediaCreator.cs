using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class MediaCreator
    {
        private readonly IMediaService _mediaService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MediaCreator>? _logger;

        public MediaCreator(
            IMediaService mediaService,
            IMediaTypeService mediaTypeService,
            MediaFileManager mediaFileManager,
            IHttpClientFactory httpClientFactory,
            ILogger<MediaCreator>? logger = null)
        {
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _mediaTypeService = mediaTypeService ?? throw new ArgumentNullException(nameof(mediaTypeService));
            _mediaFileManager = mediaFileManager ?? throw new ArgumentNullException(nameof(mediaFileManager));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
        }

        public void CreateMedia(List<YamlMedia> mediaItems, int? parentId = null, string? defaultFolder = null)
        {
            if (mediaItems == null) throw new ArgumentNullException(nameof(mediaItems));

            foreach (var yamlMedia in mediaItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlMedia.Name))
                    {
                        _logger?.LogWarning("Media entry is missing a name. Skipping.");
                        continue;
                    }

                    // [REMOVE]
                    if (yamlMedia.Remove)
                    {
                        var candidates = parentId.HasValue
                            ? _mediaService.GetPagedChildren(parentId.Value, 0, int.MaxValue, out _).ToList()
                            : _mediaService.GetRootMedia().ToList();

                        var toDelete = candidates.FirstOrDefault(m => m.Name == yamlMedia.Name);
                        if (toDelete != null)
                        {
                            _mediaService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation("Media '{Name}' removed.", yamlMedia.Name);
                        }
                        else
                        {
                            _logger?.LogDebug("Media '{Name}' not found for removal. Skipping.", yamlMedia.Name);
                        }
                        continue;
                    }

                    // [UPDATE] — update if exists, skip if not found
                    if (yamlMedia.Update)
                    {
                        var candidates = parentId.HasValue
                            ? _mediaService.GetPagedChildren(parentId.Value, 0, int.MaxValue, out _).ToList()
                            : _mediaService.GetRootMedia().ToList();

                        var toUpdate = candidates.FirstOrDefault(m => m.Name == yamlMedia.Name);
                        if (toUpdate != null)
                        {
                            if (!string.IsNullOrWhiteSpace(yamlMedia.Url))
                                TryAttachFileFromUrl(toUpdate, yamlMedia.Url);

                            SetProperties(toUpdate, yamlMedia);
                            _mediaService.Save(toUpdate, Constants.Security.SuperUserId);
                            _logger?.LogInformation("Media '{Name}' updated.", yamlMedia.Name);

                            if (yamlMedia.Children?.Any() == true)
                                CreateMedia(yamlMedia.Children, toUpdate.Id);
                        }
                        else
                        {
                            _logger?.LogInformation("Media '{Name}' not found. Skipping update.", yamlMedia.Name);
                        }
                        continue;
                    }

                    // Resolve folder: per-item folder takes precedence, then section-level defaultFolder
                    var effectiveParentId = parentId;
                    var folderToUse = !string.IsNullOrWhiteSpace(yamlMedia.Folder)
                        ? yamlMedia.Folder
                        : defaultFolder;
                    if (!string.IsNullOrWhiteSpace(folderToUse))
                        effectiveParentId = EnsureFolder(folderToUse, parentId ?? -1);

                    // Check if already exists
                    var existing = (effectiveParentId.HasValue
                        ? _mediaService.GetPagedChildren(effectiveParentId.Value, 0, int.MaxValue, out _)
                        : _mediaService.GetRootMedia()).FirstOrDefault(m => m.Name == yamlMedia.Name);

                    if (existing != null)
                    {
                        _logger?.LogInformation("Media '{Name}' already exists. Skipping.", yamlMedia.Name);
                        if (yamlMedia.Children?.Any() == true)
                            CreateMedia(yamlMedia.Children, existing.Id);
                        continue;
                    }

                    var media = _mediaService.CreateMedia(
                        yamlMedia.Name,
                        effectiveParentId ?? -1,
                        yamlMedia.MediaType,
                        Constants.Security.SuperUserId);

                    // Download file from URL if provided
                    if (!string.IsNullOrWhiteSpace(yamlMedia.Url))
                    {
                        TryAttachFileFromUrl(media, yamlMedia.Url);
                    }

                    SetProperties(media, yamlMedia);
                    _mediaService.Save(media, Constants.Security.SuperUserId);
                    _logger?.LogInformation("Media '{Name}' created.", yamlMedia.Name);

                    if (yamlMedia.Children?.Any() == true)
                        CreateMedia(yamlMedia.Children, media.Id);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing media '{Name}'.", yamlMedia.Name);
                    throw;
                }
            }
        }

        private void SetProperties(IMedia media, YamlMedia yamlMedia)
        {
            foreach (var kvp in yamlMedia.Properties)
            {
                if (media.Properties.Any(p => p.Alias == kvp.Key))
                    media.SetValue(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Resolves a folder path (e.g. "Images" or "Images/Partners") under <paramref name="rootParentId"/>,
        /// creating any missing Folder media nodes along the way. Returns the ID of the deepest folder.
        /// </summary>
        private int EnsureFolder(string folderPath, int rootParentId)
        {
            var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var currentParentId = rootParentId;

            foreach (var part in parts)
            {
                var children = currentParentId == -1
                    ? _mediaService.GetRootMedia().ToList()
                    : _mediaService.GetPagedChildren(currentParentId, 0, int.MaxValue, out _).ToList();

                var folder = children.FirstOrDefault(m =>
                    string.Equals(m.Name, part, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(m.ContentType.Alias, "Folder", StringComparison.OrdinalIgnoreCase));

                if (folder == null)
                {
                    folder = _mediaService.CreateMedia(part, currentParentId, "Folder", Constants.Security.SuperUserId);
                    _mediaService.Save(folder, Constants.Security.SuperUserId);
                    _logger?.LogInformation("Created media folder '{Name}' under parent {ParentId}.", part, currentParentId);
                }

                currentParentId = folder.Id;
            }

            return currentParentId;
        }

        private void TryAttachFileFromUrl(IMedia media, string url)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var response = client.GetAsync(url).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var fileName = Path.GetFileName(new Uri(url).LocalPath);
                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = "file";

                // Store under a media-key-based folder matching Umbraco's convention: {mediaKey:N}/{filename}
                var filePath = $"{media.Key:N}/{fileName}";

                using var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                _mediaFileManager.FileSystem.AddFile(filePath, stream, overrideIfExists: true);

                var fileUrl = _mediaFileManager.FileSystem.GetUrl(filePath);

                // The Image media type uses the ImageCropper editor which expects JSON.
                // Other types (File, Video) use the UploadField editor which stores a plain path.
                if (string.Equals(media.ContentType.Alias, Constants.Conventions.MediaTypes.Image, StringComparison.OrdinalIgnoreCase))
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        src        = fileUrl,
                        focalPoint = new { left = 0.5, top = 0.5 },
                        crops      = Array.Empty<object>()
                    });
                    media.SetValue(Constants.Conventions.Media.File, json);
                }
                else
                {
                    media.SetValue(Constants.Conventions.Media.File, fileUrl);
                }

                _logger?.LogInformation("Attached file '{FileName}' from URL to media '{Name}'.", fileName, media.Name);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to download file from '{Url}' for media '{Name}'. Continuing without file.", url, media.Name);
            }
        }
    }
}
