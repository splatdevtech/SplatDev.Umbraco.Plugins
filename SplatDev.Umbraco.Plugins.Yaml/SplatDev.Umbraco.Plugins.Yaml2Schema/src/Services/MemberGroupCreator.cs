using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class MemberGroupCreator
    {
        private readonly IMemberGroupService _memberGroupService;
        private readonly ILogger<MemberGroupCreator>? _logger;

        public MemberGroupCreator(IMemberGroupService memberGroupService, ILogger<MemberGroupCreator>? logger = null)
        {
            _memberGroupService = memberGroupService ?? throw new ArgumentNullException(nameof(memberGroupService));
            _logger = logger;
        }

        public void CreateMemberGroups(List<YamlMemberGroup> memberGroups)
        {
            if (memberGroups == null) throw new ArgumentNullException(nameof(memberGroups));

            var processedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var yaml in memberGroups)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yaml.Name))
                    {
                        _logger?.LogWarning("MemberGroup entry is missing a name. Skipping.");
                        continue;
                    }

                    if (processedNames.Contains(yaml.Name))
                    {
                        _logger?.LogWarning("MemberGroup '{Name}' is a duplicate and will be skipped.", yaml.Name);
                        continue;
                    }

                    // [REMOVE]
                    if (yaml.Remove)
                    {
                        var toDelete = _memberGroupService.GetByName(yaml.Name);
                        if (toDelete != null)
                        {
                            _memberGroupService.Delete(toDelete);
                            _logger?.LogInformation("MemberGroup '{Name}' removed.", yaml.Name);
                        }
                        else
                        {
                            _logger?.LogDebug("MemberGroup '{Name}' not found for removal. Skipping.", yaml.Name);
                        }
                        processedNames.Add(yaml.Name);
                        continue;
                    }

                    var existing = _memberGroupService.GetByName(yaml.Name);
                    if (existing != null)
                    {
                        _logger?.LogInformation("MemberGroup '{Name}' already exists. Skipping.", yaml.Name);
                        processedNames.Add(yaml.Name);
                        continue;
                    }

                    var group = new MemberGroup { Name = yaml.Name };
                    _memberGroupService.Save(group);
                    _logger?.LogInformation("MemberGroup '{Name}' created.", yaml.Name);
                    processedNames.Add(yaml.Name);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing MemberGroup '{Name}'.", yaml.Name);
                    throw;
                }
            }
        }
    }
}
