using FormBuilder.Core.Services.Interfaces;

using System.Collections;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
    internal sealed class PublishedContentPageService(
      IUmbracoContextFactory umbracoContextFactory,
      IPublishedValueFallback publishedValueFallback) : IPageService
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory = umbracoContextFactory;
        private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;

        internal static readonly string[] sourceArray =
        [
            "Umbraco.RichText"
        ];

        public Hashtable GetPageElements() => GetPageElements(GetContent());

        public Hashtable GetPageElements(int contentId) => GetPageElements(GetContent(new int?(contentId)));

        private IPublishedContent? GetContent(int? contentId = null)
        {
            using UmbracoContextReference contextReference = _umbracoContextFactory.EnsureUmbracoContext();
            if (!contentId.HasValue)
                contentId = contextReference.UmbracoContext.PublishedRequest?.PublishedContent?.Id;
            return contentId.HasValue ? GetContent(contextReference.UmbracoContext.Content, contentId.Value) : null;
        }

        private static IPublishedContent? GetContent(
          IPublishedContentCache? contentCache,
          int contentId)
        {
            return contentCache?.GetById(contentId);
        }

        private Hashtable GetPageElements(IPublishedContent? content)
        {
            Hashtable pageElements = [];
            if (content is not null)
            {
                pageElements.Add("pageID", content.Id);
                Hashtable hashtable = pageElements;
#pragma warning disable CS0618 // Type or member is obsolete
                IPublishedContent? parent = content.Parent;
#pragma warning restore CS0618 // Type or member is obsolete
                int local = parent is not null ? parent.Id : -1;
                hashtable.Add("parentID", local);
                pageElements.Add("pageName", content.Name);
                pageElements.Add("nodeType", content.ContentType.Id);
                pageElements.Add("nodeTypeAlias", content.ContentType.Alias);
                pageElements.Add("writerName", content.WriterName());
                pageElements.Add("creatorName", content.CreatorName());
                pageElements.Add("createDate", content.CreateDate);
                pageElements.Add("updateDate", content.UpdateDate);
                pageElements.Add("path", content.Path);
                pageElements.Add("splitpath", content.Path.Split(
                [
                    ','
                ]));
                int? templateId = content.TemplateId;
                if (templateId.HasValue)
                {
                    int valueOrDefault = templateId.GetValueOrDefault();
                    pageElements.Add("template", valueOrDefault);
                }
                foreach (IPublishedProperty property in content.Properties.Where(new Func<IPublishedProperty, bool>(IncludePropertyForPageElements)))
                {
                    if (!pageElements.ContainsKey(property.Alias))
                        pageElements.Add(property.Alias, property.Value(_publishedValueFallback));
                }
            }
            return pageElements;
        }

        private bool IncludePropertyForPageElements(IPublishedProperty property)
        {
            return !sourceArray.Contains(property.PropertyType.EditorAlias);
        }
    }
}