namespace SplatDev.Umbraco.Plugins.CodeFirst.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;

    using SplatDev.Umbraco.Plugins.CodeFirst.Helpers;
    using SplatDev.Umbraco.Plugins.CodeFirst.Models;

    [Obsolete("SplatDev.Umbraco.Plugins.CodeFirst is deprecated. Use SplatDev.Umbraco.Plugins.Yaml2Schema instead. This package is maintained for backwards compatibility only.")]
    public class LocalizationService
    {
        private readonly IAuditService auditService;
        private readonly IContentService contentService;
        private readonly IDomainService domainService;
        // TODO: UmbracoHelper is no longer injected directly in Umbraco 13+; use IUmbracoContextAccessor instead.
        private readonly IUmbracoContextAccessor umbracoContextAccessor;
        private readonly ILocalizationService localizationService;
        private readonly ILogger<LocalizationService> logger;

        // TODO: Replace UmbracoHelper with IUmbracoContextAccessor for Umbraco 13+ compatibility.
        public LocalizationService(
            ILocalizationService localizationService,
            IDomainService domainService,
            IContentService contentService,
            IAuditService auditService,
            IUmbracoContextAccessor umbracoContextAccessor,
            ILogger<LocalizationService> logger)
        {
            this.localizationService = localizationService;
            this.domainService = domainService;
            this.contentService = contentService;
            this.auditService = auditService;
            this.umbracoContextAccessor = umbracoContextAccessor;
            this.logger = logger;
        }

        public void AddCultureAndHostname(int rootId, CultureAndHostname[] cultureAndHostnames)
        {
            const string protocolBase = "http";
            try
            {
                var root = contentService.GetById(rootId);
                var languages = localizationService.GetAllLanguages();

                foreach (var cult in cultureAndHostnames)
                {
                    var protocol = cult.SecureProtocol ? $"{protocolBase}s" : protocolBase;
                    string domainName = $"{protocol}://";
                    domainName += !string.IsNullOrEmpty(cult.Subdomain) ? $"{cult.Subdomain}." : "";
                    domainName += $"{cult.Domain}/";

                    var existingDomain = domainService.GetByName(domainName);
                    if (Uri.IsWellFormedUriString(domainName, UriKind.RelativeOrAbsolute))
                    {
                        if (existingDomain == null)
                        {
                            var defaultLanguageId = localizationService.GetDefaultLanguageId();
                            existingDomain = new UmbracoDomain(domainName)
                            {
                                LanguageId = defaultLanguageId,
                                RootContentId = rootId
                            };

                            domainService.Save(existingDomain);
                            auditService.Add(AuditType.New, -1, existingDomain.Id, "Language", $"Domain '{domainName}' for root '{root!.Name}' has been created");
                        }

                        foreach (var language in languages)
                        {
                            var localizedDomainName = $"{domainName}{language.IsoCode}";
                            var localizedDomain = domainService.GetByName(localizedDomainName);
                            if (localizedDomain == null)
                            {
                                localizedDomain = new UmbracoDomain(localizedDomainName)
                                {
                                    LanguageId = language.Id,
                                    RootContentId = rootId
                                };
                                domainService.Save(localizedDomain);
                                auditService.Add(AuditType.New, -1, existingDomain.Id, "Language", $"Domain '{localizedDomainName}' for root '{root!.Name}' has been created");

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddCultureAndHostname for root {RootId}", rootId);
                throw;
            }
        }

        public void ClearCulturesAndHostnames(int rootId)
        {
            foreach (var domain in domainService.GetAssignedDomains(rootId, true))
            {
                domainService.Delete(domain);
                auditService.Add(AuditType.Save, -1, domain.Id, "Domain", $"Cultures and Hostnames for {rootId} have been cleared");
            }
        }

        public void DeleteCultureAndHostname(int rootId, CultureAndHostname[] cultureAndHostnames)
        {
            const string protocolBase = "http";
            try
            {
                var root = contentService.GetById(rootId);
                var languages = localizationService.GetAllLanguages();

                foreach (var cult in cultureAndHostnames)
                {
                    var protocol = cult.SecureProtocol ? $"{protocolBase}s" : protocolBase;
                    string domainName = $"{protocol}://";
                    domainName += !string.IsNullOrEmpty(cult.Subdomain) ? $"{cult.Subdomain}." : "";
                    domainName += $"{cult.Domain}/";

                    var existingDomain = domainService.GetByName(domainName);

                    if (existingDomain != null)
                    {
                        domainService.Delete(existingDomain);
                        auditService.Add(AuditType.Delete, -1, existingDomain.Id, "Language", $"Domain '{domainName}' for root '{root!.Name}' has been deleted");
                    }

                    foreach (var language in languages)
                    {
                        var localizedDomainName = $"{domainName}{language.IsoCode}";
                        var localizedDomain = domainService.GetByName(localizedDomainName);
                        if (localizedDomain != null)
                        {
                            domainService.Delete(localizedDomain);
                            auditService.Add(AuditType.Delete, -1, localizedDomain.Id, "Language", $"Domain '{localizedDomainName}' for root '{root!.Name}' has been deleted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DeleteCultureAndHostname for root {RootId}", rootId);
                throw;
            }
        }

        public void DeleteMissingCultureAndHostname(int rootId)
        {
            var domains = domainService.GetAssignedDomains(rootId, false);
            var languages = localizationService.GetAllLanguages();
            foreach (var domain in domains)
            {
                var lang = domain.LanguageIsoCode;
                if (languages.SingleOrDefault(x => x.IsoCode.Equals(lang)) == null)
                {
                    domainService.Delete(domain);
                }
            }
        }

        public IReadOnlyDictionary<string, PublishedCultureInfo>? GetCulturesAndHostnames(int nodeId)
        {
            // TODO: UmbracoHelper.Content() replaced by IUmbracoContextAccessor in Umbraco 13+
            if (umbracoContextAccessor.TryGetUmbracoContext(out var ctx))
            {
                var root = ctx.Content?.GetById(nodeId);
                return root?.Cultures;
            }
            return null;
        }

        public IEnumerable<IDomain> GetDomains(int nodeId)
        {
            return domainService.GetAssignedDomains(nodeId, true);
        }

        public IEnumerable<string> GetDomainStrings(int nodeId)
        {
            var domains = GetDomains(nodeId);
            return domains.Select(x => x.DomainName);
        }

        public bool HasLanguage(string isoCode)
        {
            var languages = localizationService.GetAllLanguages().Select(x => x.IsoCode).ToList();
            return languages.Contains(isoCode);
        }

        public void SetAllNodeVariants(int nodeId, string defaultName)
        {
            var home = contentService.GetById(nodeId);
            var children = contentService.GetByLevel(2).Where(x => x.ParentId == nodeId);
            try
            {
                foreach (var language in localizationService.GetAllLanguages())
                {
                    if (home!.GetCultureName(language.IsoCode) == null)
                    {
                        home.SetCultureName(defaultName, language.IsoCode);
                    }

                    foreach (var child in children)
                    {
                        if (child.GetCultureName(language.IsoCode) == null)
                        {
                            child.SetCultureName($"{defaultName}-{language}", language.IsoCode);
                            contentService.Save(child);
                        }
                    }
                }
                auditService.Add(AuditType.Save, -1, home!.Id, "Content Node", $"Children for {home.Name} have been refreshed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SetAllNodeVariants for node {NodeId}", nodeId);
                throw;
            }
        }
    }
}
