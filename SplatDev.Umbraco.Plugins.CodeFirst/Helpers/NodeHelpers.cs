namespace SplatDev.Umbraco.Plugins.CodeFirst.Helpers
{
    using System;
    using Examine;
    using System.Linq;
    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Newtonsoft.Json.Linq;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Services;
    using System.Collections.Generic;
    using SplatDev.Umbraco.Plugins.CodeFirst.Models;
    using Umbraco.Cms.Core.PropertyEditors;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.Web;

    /// <summary>
    /// CMS Helper is a helper for various funcionalities of the site
    /// </summary>
    public class NodeHelpers
    {
        private readonly IAuditService auditService;
        private readonly IContentService contentService;
        private readonly IContentTypeService contentTypeService;
        private readonly IDomainService? domainService;
        private readonly ILocalizationService localizationService;
        private readonly ILogger logger;
        private readonly ISearcher? searcher;

        // TODO: DI wiring - inject all required services. ISqlContext removed in Umbraco 13+.
        public NodeHelpers(
            IAuditService auditService,
            IContentService contentService,
            IContentTypeService contentTypeService,
            ILocalizationService localizationService,
            IDomainService? domainService,
            ILogger logger,
            IExamineManager? examineManager = null)
        {
            this.auditService = auditService;
            this.contentService = contentService;
            this.contentTypeService = contentTypeService;
            this.localizationService = localizationService;
            this.domainService = domainService;
            this.logger = logger;

            if (examineManager != null && examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex? index))
            {
                searcher = index.GetSearcher();
            }
        }

        public IContent? Root(string documentTypeAlias = "")
        {
            var roots = contentService.GetRootContent();

            if (!string.IsNullOrEmpty(documentTypeAlias))
                return roots.FirstOrDefault(x => x.ContentType.Alias == documentTypeAlias);

            return roots.FirstOrDefault();
        }

        public IEnumerable<IContent> Roots => contentService.GetRootContent();

        public void AddNodeDomain(int nodeId, string domain, string language = "en-Us")
        {
            var newDomain = new UmbracoDomain(domain);
            var node = contentService.GetById(nodeId);
            var lang = localizationService.GetLanguageIdByIsoCode(language);
            newDomain.LanguageId = lang.HasValue ? lang.Value : (int?)null;
            newDomain.RootContentId = node!.Id;
            domainService?.Save(newDomain);
        }

        public IContent AddNodeVariation(int nodeId, string language, Dictionary<string, object>? data = null)
        {
            IContent node = contentService.GetById(nodeId)!;
            node.SetCultureName($"{node.Name}-{language}", language);
            if (data != null) SetPropertiesValues(node, data);

            return node;
        }

        public void AddVariationToChildrenOf(int nodeId, string[] languages)
        {
            var children = contentService.GetByLevel(2).Where(x => x.ParentId == nodeId);
            try
            {
                foreach (var language in languages)
                {
                    foreach (var child in children)
                    {
                        if (child.GetCultureName(language) == null)
                        {
                            child.SetCultureName($"{child.Name}-{language}", language);
                            contentService.Save(child);
                        }
                    }
                }
                auditService.Add(AuditType.Save, -1, nodeId, "Content Node", $"Children for {nodeId} have been updated with new language");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding variation to children of {NodeId}", nodeId);
                throw;
            }
        }

        public IContent? CreateNode(int parentId, string documentTypeAliasAlias, string nodeName, Dictionary<string, object>? data = null)
        {
            try
            {
                IContent node = contentService.Create(nodeName, parentId, documentTypeAliasAlias);
                if (data != null) SetPropertiesValues(node, data);

                return node;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating node {NodeName}", nodeName);
                throw;
            }
        }

        public void DeleteNode(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            contentService.Delete(node!);
        }

        public IEnumerable<IContent> GetAllRoots()
        {
            return contentService.GetByLevel(1);
        }

        public IEnumerable<IPublishedContent> GetDescendants(IPublishedContent node, string parentDocumentTypeAlias, string documentTypeAlias)
        {
            IPublishedContent? list = node
            .Descendants(parentDocumentTypeAlias)
            .FirstOrDefault();

            return list!.DescendantsOfType(documentTypeAlias);
        }

        public List<IPublishedContent> GetDescendants(IPublishedContent node)
        {
            return node.Descendants().ToList();
        }

        public IEnumerable<IPublishedContent> GetDescendants(IPublishedContent node, string documentTypeAlias)
        {
            return node.Descendants(documentTypeAlias).ToList();
        }

        public IEnumerable<IContent> GetDescendants(int nodeId)
        {
            return contentService.GetPagedChildren(nodeId, 0, int.MaxValue, out long _);
        }

        public IContent? GetNodeOfType(string documentTypeAlias, int parentId)
        {
            // TODO: ISqlContext / IQuery<IContent> was Umbraco 8 pattern. In Umbraco 13+, use
            // contentService.GetPagedChildren directly or search via IExamineManager.
            return contentService.GetPagedChildren(parentId, 0, int.MaxValue, out long _)
                    .FirstOrDefault(x => x.ContentType.Alias == documentTypeAlias);
        }

        public IContent? GetNodeOfType(string documentTypeAlias, string nodeName)
        {
            if (searcher != null)
            {
                var fieldToSearch = "name";
                var criteria = searcher.CreateQuery().NodeTypeAlias(documentTypeAlias).And().Field(fieldToSearch, nodeName).Execute();
                if (criteria.TotalItemCount >= 1)
                {
                    var nodeId = int.Parse(criteria.FirstOrDefault()!.Values["Id"]);
                    return contentService.GetById(nodeId);
                }
            }
            return null;
        }

        public IEnumerable<IContent> GetDescendantsOfType(string documentTypeAlias, int nodeId)
        {
            // TODO: IQuery<IContent> was Umbraco 8 pattern; filter after retrieval in Umbraco 13+
            return contentService.GetPagedOfType(nodeId, 0, int.MaxValue, out long _)
                .Where(x => x.ContentType.Alias == documentTypeAlias);
        }

        public IEnumerable<IContent> GetDescendantsOfType(string documentTypeAlias, string parentDocumentTypeAlias, string parentName = "")
        {
            if (searcher == null) return Enumerable.Empty<IContent>();

            var criteria = searcher.CreateQuery().NodeTypeAlias(parentDocumentTypeAlias);
            if (!string.IsNullOrEmpty(parentName))
            {
                var fieldToSearch = "name";
                criteria.And().Field(fieldToSearch, parentDocumentTypeAlias);
            }
            var results = criteria.Execute();

            var descendants = new List<IContent>();

            foreach (var result in results)
            {
                var nodeId = int.Parse(result.Values["Id"]);
                IEnumerable<ISearchResult> newCriteria = searcher.CreateQuery().NodeTypeAlias(documentTypeAlias).And().Field("parentId", nodeId).Execute();
                foreach (var node in newCriteria)
                {
                    var content = contentService.GetById(int.Parse(node.Id));
                    if (content != null) descendants.Add(content);
                }
            }

            return descendants;
        }

        public IContent? GetNode(int id)
        {
            return contentService.GetById(id);
        }

        public IPublishedContent? GetNode(IPublishedContent node, string documentTypeAlias)
        {
            var result = node
            .Descendants(documentTypeAlias)
            .FirstOrDefault();

            return result;
        }

        public IEnumerable<IDomain> GetNodeDomains(int nodeId)
        {
            return domainService?.GetAssignedDomains(nodeId, true) ?? Enumerable.Empty<IDomain>();
        }

        public string GetNodeGuid(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            return node!.Key.ToString().Replace("-", string.Empty);
        }

        public IEnumerable<ILanguage> GetNodeLanguages()
        {
            return localizationService.GetAllLanguages();
        }

        public GuidUdi GetNodeUdi(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            if (node == null)
            {
                // Return a root placeholder UDI when node doesn't exist (e.g. parentId == -1)
                return new GuidUdi(Constants.UdiEntityType.Document, Guid.Empty);
            }
            var udi = new GuidUdi(node.ContentType.GetUdi().ToString(), node.Key);
            return udi;
        }

        public GuidUdi GetNodeUdi(IPublishedContent node)
        {
            var udi = new GuidUdi(node.ContentType.ItemType.ToString(), node.Key);
            return udi;
        }

        public IEnumerable<PreValue> GetPreValue(IDataTypeService dataTypeService, string propertyName)
        {
            var dataType = dataTypeService.GetDataType(propertyName);
            var prevalues = new List<PreValue>();
            if (dataType != null)
            {
                ValueListConfiguration valueList = (ValueListConfiguration)dataType.Configuration!;

                if (valueList != null && valueList.Items != null && valueList.Items.Any())
                {
                    prevalues.AddRange(valueList.Items.Select(s => new PreValue
                    {
                        Id = s.Id,
                        Value = s.Value
                    }));
                }
            }
            return prevalues;
        }

        public IContent? GetRootByDocumentType(string documentTypeAliasAlias)
        {
            var root = contentService.GetRootContent().SingleOrDefault(x => x.ContentType.Alias == documentTypeAliasAlias);
            return root;
        }

        public string GetScrollId(IPublishedContent node, string documentTypeAlias, string navigationId = "")
        {
            navigationId = string.IsNullOrEmpty(navigationId) ? "Navigation_Scroll_Id" : navigationId;
            dynamic? page = GetNode(node, documentTypeAlias);
            if (page != null && page.HasValue(navigationId)) return page.GetPropertyValue(navigationId);
            else return "";
        }

        public void PublishNode(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            auditService.Add(AuditType.Publish, -1, node!.Id, "Content Node", $" Home '{node.Name}' has been published");
            contentService.SaveAndPublish(node);
        }

        public bool RootExists(string documentTypeAliasAlias)
        {
            var root = GetRootByDocumentType(documentTypeAliasAlias);
            return root != null;
        }

        public void SaveNode(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            SaveNode(node!);
        }

        public void SaveNode(IContent node)
        {
            auditService.Add(AuditType.New, -1, node.Id, "Content Node", $"Node '{node.Name}' has been created programatically.");
            contentService.Save(node);
        }

        public IContent SetPropertiesValues(IContent node, Dictionary<string, object> data)
        {
            foreach (var datum in data)
            {
                node.SetValue(datum.Key, datum.Value);
            }
            return node;
        }

        public IContent SetPropertyValue(int nodeId, string propertyAlias, object value, bool hasVariations = true)
        {
            var node = contentService.GetById(nodeId)!;
            return SetPropertyValue(node, propertyAlias, value, hasVariations);
        }

        public IContent SetPropertyValue(IContent node, string propertyAlias, object value, bool hasVariations = true)
        {
            if (hasVariations)
            {
                var languages = localizationService.GetAllLanguages();
                foreach (var language in languages)
                {
                    node.SetValue(propertyAlias, value, language.IsoCode);
                }
            }
            else
            {
                node.SetValue(propertyAlias, value);
            }

            return node;
        }

        public void TryPublishSite(IUmbracoContextFactory contextFactory, int rootId)
        {
            try
            {
                using (contextFactory.EnsureUmbracoContext())
                {
                    var home = contentService.GetById(rootId);
                    contentService.SaveAndPublishBranch(home!, true);
                    auditService.Add(AuditType.Publish, -1, home!.Id, "Content Node", $"Home '{home.Name}' has been published");
                }
            }
            catch
            {
                //ignore because of possible bug https://github.com/umbraco/Umbraco-CMS/issues/5281
            }
        }

        public void UnpublishNode(int nodeId)
        {
            var node = contentService.GetById(nodeId);
            auditService.Add(AuditType.Publish, -1, node!.Id, "Content Node", $"Home '{node.Name}' has been unpublished");
            contentService.Unpublish(node);
        }
    }
}
