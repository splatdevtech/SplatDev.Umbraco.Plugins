using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Models;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Services
{
    public class MemberCreator
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<MemberCreator>? _logger;

        public MemberCreator(IMemberService memberService, ILogger<MemberCreator>? logger = null)
        {
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger;
        }

        public void CreateMembers(List<YamlMember> members)
        {
            if (members == null) throw new ArgumentNullException(nameof(members));

            var processedAliases = new HashSet<string>();

            foreach (var yamlMember in members)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(yamlMember.Email))
                    {
                        _logger?.LogWarning("Member entry is missing an email. Skipping.");
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(yamlMember.Alias) && processedAliases.Contains(yamlMember.Alias))
                    {
                        _logger?.LogWarning("Member with alias '{Alias}' is a duplicate and will be skipped.", yamlMember.Alias);
                        continue;
                    }

                    // [REMOVE]
                    if (yamlMember.Remove)
                    {
                        var toDelete = _memberService.GetByEmail(yamlMember.Email);
                        if (toDelete != null)
                        {
                            _memberService.Delete(toDelete);
                            _logger?.LogInformation("Member '{Email}' removed.", yamlMember.Email);
                        }
                        else
                        {
                            _logger?.LogDebug("Member '{Email}' not found for removal. Skipping.", yamlMember.Email);
                        }
                        if (!string.IsNullOrWhiteSpace(yamlMember.Alias))
                            processedAliases.Add(yamlMember.Alias);
                        continue;
                    }

                    var existing = _memberService.GetByEmail(yamlMember.Email);

                    // [UPDATE]
                    if (yamlMember.Update && existing != null)
                    {
                        existing.Name = yamlMember.Name;
                        existing.IsApproved = yamlMember.IsApproved;
                        SetProperties(existing, yamlMember);
                        _memberService.Save(existing);
                        _logger?.LogInformation("Member '{Email}' updated.", yamlMember.Email);
                        if (!string.IsNullOrWhiteSpace(yamlMember.Alias))
                            processedAliases.Add(yamlMember.Alias);
                        continue;
                    }

                    if (existing != null)
                    {
                        _logger?.LogInformation("Member '{Email}' already exists. Skipping.", yamlMember.Email);
                        if (!string.IsNullOrWhiteSpace(yamlMember.Alias))
                            processedAliases.Add(yamlMember.Alias);
                        continue;
                    }

                    // Create
                    var member = _memberService.CreateMember(
                        yamlMember.Username ?? yamlMember.Email,
                        yamlMember.Email,
                        yamlMember.Name,
                        yamlMember.MemberType);

                    member.IsApproved = yamlMember.IsApproved;
                    SetProperties(member, yamlMember);

                    _memberService.Save(member);

                    if (!string.IsNullOrWhiteSpace(yamlMember.Password))
                    {
                        _logger?.LogWarning(
                            "Password for member '{Email}' cannot be set via IMemberService in Umbraco 17. Set it manually or via the identity provider.",
                            yamlMember.Email);
                    }

                    _logger?.LogInformation("Member '{Email}' created.", yamlMember.Email);

                    if (!string.IsNullOrWhiteSpace(yamlMember.Alias))
                        processedAliases.Add(yamlMember.Alias);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing member '{Email}'.", yamlMember.Email);
                    throw;
                }
            }
        }

        private void SetProperties(IMember member, YamlMember yaml)
        {
            foreach (var kvp in yaml.Properties)
            {
                if (member.Properties.Any(p => p.Alias == kvp.Key))
                    member.SetValue(kvp.Key, kvp.Value);
            }
        }
    }
}
