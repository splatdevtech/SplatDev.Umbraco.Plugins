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
    public class MemberTypeCreator
    {
        private readonly IMemberTypeService _memberTypeService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ILogger<MemberTypeCreator>? _logger;

        public MemberTypeCreator(
            IMemberTypeService memberTypeService,
            IDataTypeService dataTypeService,
            IShortStringHelper shortStringHelper,
            ILogger<MemberTypeCreator>? logger = null)
        {
            _memberTypeService = memberTypeService ?? throw new ArgumentNullException(nameof(memberTypeService));
            _dataTypeService = dataTypeService ?? throw new ArgumentNullException(nameof(dataTypeService));
            _shortStringHelper = shortStringHelper ?? throw new ArgumentNullException(nameof(shortStringHelper));
            _logger = logger;
        }

        public void CreateMemberTypes(List<YamlMemberType> memberTypes, List<YamlDataType> dataTypes = null)
        {
            if (memberTypes == null) throw new ArgumentNullException(nameof(memberTypes));

            var dataTypeNameByAlias = dataTypes?
                .Where(d => !string.IsNullOrEmpty(d.Alias) && !string.IsNullOrEmpty(d.Name))
                .GroupBy(d => d.Alias, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Name, StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var processedAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var yaml in memberTypes)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yaml.Alias))
                    {
                        _logger?.LogWarning("MemberType entry is missing an alias. Skipping.");
                        continue;
                    }

                    if (processedAliases.Contains(yaml.Alias))
                    {
                        _logger?.LogWarning("MemberType with alias '{Alias}' is a duplicate and will be skipped.", yaml.Alias);
                        continue;
                    }

                    // [REMOVE]
                    if (yaml.Remove)
                    {
                        var toDelete = _memberTypeService.Get(yaml.Alias);
                        if (toDelete != null)
                        {
                            _memberTypeService.Delete(toDelete, Constants.Security.SuperUserId);
                            _logger?.LogInformation("MemberType '{Alias}' removed.", yaml.Alias);
                        }
                        else
                        {
                            _logger?.LogDebug("MemberType '{Alias}' not found for removal. Skipping.", yaml.Alias);
                        }
                        processedAliases.Add(yaml.Alias);
                        continue;
                    }

                    var existing = _memberTypeService.Get(yaml.Alias);

                    // [UPDATE]
                    if (yaml.Update && existing != null)
                    {
                        existing.Name = yaml.Name;
                        if (!string.IsNullOrWhiteSpace(yaml.Description))
                            existing.Description = yaml.Description;
                        if (!string.IsNullOrWhiteSpace(yaml.Icon))
                            existing.Icon = yaml.Icon;
                        MergeTabsAndProperties(existing, yaml, dataTypeNameByAlias);
                        _memberTypeService.Save(existing);
                        _logger?.LogInformation("MemberType '{Alias}' updated.", yaml.Alias);
                        processedAliases.Add(yaml.Alias);
                        continue;
                    }

                    if (existing != null)
                    {
                        _logger?.LogInformation("MemberType '{Alias}' already exists. Skipping.", yaml.Alias);
                        processedAliases.Add(yaml.Alias);
                        continue;
                    }

                    // Create
                    var memberType = new MemberType(_shortStringHelper, -1)
                    {
                        Name = yaml.Name,
                        Alias = yaml.Alias,
                        Icon = yaml.Icon ?? "icon-user",
                        Description = yaml.Description ?? string.Empty
                    };

                    AddTabsAndProperties(memberType, yaml, dataTypeNameByAlias);

                    _memberTypeService.Save(memberType);
                    _logger?.LogInformation("MemberType '{Name}' with alias '{Alias}' created.", yaml.Name, yaml.Alias);
                    processedAliases.Add(yaml.Alias);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing MemberType '{Alias}'.", yaml.Alias);
                    throw;
                }
            }
        }

        private void AddTabsAndProperties(MemberType memberType, YamlMemberType yaml, Dictionary<string, string> dataTypeNameByAlias)
        {
            foreach (var tab in yaml.Tabs ?? [])
            {
                var tabAlias = _shortStringHelper.CleanStringForSafeAlias(tab.Name);
                var group = new PropertyGroup(false)
                {
                    Name = tab.Name,
                    Alias = tabAlias,
                    SortOrder = tab.SortOrder
                };

                foreach (var prop in tab.Properties)
                {
                    var dtName = dataTypeNameByAlias.TryGetValue(prop.DataType, out var mapped) ? mapped : prop.DataType;
                    var dataType = _dataTypeService.GetDataType(dtName);
                    if (dataType == null)
                    {
                        _logger?.LogWarning(
                            "DataType '{DataType}' not found. Skipping property '{Alias}' in MemberType '{MemberTypeAlias}'.",
                            prop.DataType, prop.Alias, yaml.Alias);
                        continue;
                    }

                    var propType = new PropertyType(_shortStringHelper, dataType)
                    {
                        Alias = prop.Alias,
                        Name = prop.Name,
                        Mandatory = prop.Required,
                        Description = prop.Description ?? string.Empty,
                        SortOrder = prop.SortOrder,
                        ValidationRegExp = prop.ValidationRegExp ?? string.Empty
                    };

                    group.PropertyTypes!.Add(propType);
                }

                memberType.PropertyGroups.Add(group);

                // Apply member-specific flags after the property is added to the type
                foreach (var prop in tab.Properties.Where(p => p.MemberCanEdit || p.ShowOnProfile || p.IsSensitive))
                {
                    if (prop.MemberCanEdit)
                        memberType.SetMemberCanEditProperty(prop.Alias, true);
                    if (prop.ShowOnProfile)
                        memberType.SetMemberCanViewProperty(prop.Alias, true);
                    if (prop.IsSensitive)
                        memberType.SetIsSensitiveProperty(prop.Alias, true);
                }
            }
        }

        private void MergeTabsAndProperties(IMemberType existing, YamlMemberType yaml, Dictionary<string, string> dataTypeNameByAlias)
        {
            foreach (var tab in yaml.Tabs ?? [])
            {
                var tabAlias = _shortStringHelper.CleanStringForSafeAlias(tab.Name);
                var existingTab = existing.PropertyGroups.FirstOrDefault(g => g.Alias == tabAlias);

                if (existingTab == null)
                {
                    var newGroup = new PropertyGroup(false)
                    {
                        Name = tab.Name,
                        Alias = tabAlias,
                        SortOrder = tab.SortOrder
                    };

                    foreach (var prop in tab.Properties)
                    {
                        var dtName = dataTypeNameByAlias.TryGetValue(prop.DataType, out var mapped) ? mapped : prop.DataType;
                        var dataType = _dataTypeService.GetDataType(dtName);
                        if (dataType == null) continue;

                        newGroup.PropertyTypes!.Add(new PropertyType(_shortStringHelper, dataType)
                        {
                            Alias = prop.Alias,
                            Name = prop.Name,
                            Mandatory = prop.Required,
                            Description = prop.Description ?? string.Empty,
                            SortOrder = prop.SortOrder,
                            ValidationRegExp = prop.ValidationRegExp ?? string.Empty
                        });
                    }

                    existing.PropertyGroups.Add(newGroup);
                }
                else
                {
                    foreach (var prop in tab.Properties)
                    {
                        if (existingTab.PropertyTypes?.Any(p => p.Alias == prop.Alias) == true)
                            continue;

                        var dtName = dataTypeNameByAlias.TryGetValue(prop.DataType, out var mapped) ? mapped : prop.DataType;
                        var dataType = _dataTypeService.GetDataType(dtName);
                        if (dataType == null) continue;

                        existingTab.PropertyTypes!.Add(new PropertyType(_shortStringHelper, dataType)
                        {
                            Alias = prop.Alias,
                            Name = prop.Name,
                            Mandatory = prop.Required,
                            Description = prop.Description ?? string.Empty,
                            SortOrder = prop.SortOrder,
                            ValidationRegExp = prop.ValidationRegExp ?? string.Empty
                        });
                    }
                }

                foreach (var prop in tab.Properties.Where(p => p.MemberCanEdit || p.ShowOnProfile || p.IsSensitive))
                {
                    if (prop.MemberCanEdit)
                        existing.SetMemberCanEditProperty(prop.Alias, true);
                    if (prop.ShowOnProfile)
                        existing.SetMemberCanViewProperty(prop.Alias, true);
                    if (prop.IsSensitive)
                        existing.SetIsSensitiveProperty(prop.Alias, true);
                }
            }
        }
    }
}
