using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class MediaTypeCreator
    {
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ILogger<MediaTypeCreator>? _logger;

        public MediaTypeCreator(
            IMediaTypeService mediaTypeService,
            IDataTypeService dataTypeService,
            IShortStringHelper shortStringHelper,
            ILogger<MediaTypeCreator>? logger = null)
        {
            _mediaTypeService = mediaTypeService ?? throw new ArgumentNullException(nameof(mediaTypeService));
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
            _shortStringHelper = shortStringHelper ?? throw new ArgumentNullException(nameof(shortStringHelper));
            _logger = logger;
        }

        public void CreateMediaTypes(List<YamlMediaType> mediaTypes)
        {
            if (mediaTypes == null) throw new ArgumentNullException(nameof(mediaTypes));

            var processedAliases = new HashSet<string>();

            foreach (var yamlMediaType in mediaTypes)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlMediaType.Alias))
                    {
                        _logger?.LogWarning("MediaType entry is missing an alias. Skipping.");
                        continue;
                    }

                    if (processedAliases.Contains(yamlMediaType.Alias))
                    {
                        _logger?.LogWarning("MediaType '{Alias}' is a duplicate and will be skipped.", yamlMediaType.Alias);
                        continue;
                    }

                    // [REMOVE]
                    if (yamlMediaType.Remove)
                    {
                        var toDelete = _mediaTypeService.Get(yamlMediaType.Alias);
                        if (toDelete != null)
                        {
                            _mediaTypeService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation("MediaType '{Alias}' removed.", yamlMediaType.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug("MediaType '{Alias}' not found for removal. Skipping.", yamlMediaType.Alias);
                        }
                        processedAliases.Add(yamlMediaType.Alias);
                        continue;
                    }

                    var existing = _mediaTypeService.Get(yamlMediaType.Alias);

                    // [UPDATE]
                    if (yamlMediaType.Update && existing != null)
                    {
                        existing.Name = yamlMediaType.Name;
                        existing.Icon = yamlMediaType.Icon ?? "icon-picture";
                        existing.AllowedAsRoot = yamlMediaType.AllowedAtRoot;
                        _mediaTypeService.Save(existing);
                        _logger?.LogInformation("MediaType '{Alias}' updated.", yamlMediaType.Alias);
                        processedAliases.Add(yamlMediaType.Alias);
                        continue;
                    }

                    if (existing != null)
                    {
                        _logger?.LogInformation("MediaType '{Alias}' already exists. Skipping.", yamlMediaType.Alias);
                        processedAliases.Add(yamlMediaType.Alias);
                        continue;
                    }

                    // Create
                    var mediaType = new MediaType(_shortStringHelper, -1)
                    {
                        Name = yamlMediaType.Name,
                        Alias = yamlMediaType.Alias,
                        Icon = yamlMediaType.Icon ?? "icon-picture",
                        AllowedAsRoot = yamlMediaType.AllowedAtRoot
                    };

                    foreach (var tab in yamlMediaType.Tabs)
                    {
                        var tabAlias = _shortStringHelper.CleanStringForSafeAlias(tab.Name);
                        var contentTab = new PropertyGroup(false) { Name = tab.Name, Alias = tabAlias };

                        foreach (var property in tab.Properties)
                        {
                            var dataType = _dataTypeService.GetDataType(property.DataType);
                            if (dataType == null)
                            {
                                _logger?.LogWarning(
                                    "DataType '{DataType}' not found. Skipping property '{PropertyAlias}' in MediaType '{Alias}'.",
                                    property.DataType, property.Alias, yamlMediaType.Alias);
                                continue;
                            }

                            contentTab.PropertyTypes!.Add(new PropertyType(_shortStringHelper, dataType)
                            {
                                Alias = property.Alias,
                                Name = property.Name,
                                Mandatory = property.Required,
                                Description = property.Description
                            });
                        }

                        mediaType.PropertyGroups.Add(contentTab);
                    }

                    _mediaTypeService.Save(mediaType);
                    _logger?.LogInformation("MediaType '{Alias}' created.", yamlMediaType.Alias);
                    processedAliases.Add(yamlMediaType.Alias);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing MediaType '{Alias}'.", yamlMediaType.Alias);
                    throw;
                }
            }
        }
    }
}
