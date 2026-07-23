
// Type: Umbraco.Forms.Core.Services.PublishedContentPageService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal sealed class PublishedContentPageService : IPageService
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IPublishedValueFallback _publishedValueFallback;

        public PublishedContentPageService(
          IUmbracoContextFactory umbracoContextFactory,
          IPublishedValueFallback publishedValueFallback)
        {
            this._umbracoContextFactory = umbracoContextFactory;
            this._publishedValueFallback = publishedValueFallback;
        }

        public Hashtable GetPageElements() => this.GetPageElements(this.GetContent());

        public Hashtable GetPageElements(int contentId) => this.GetPageElements(this.GetContent(new int?(contentId)));

        private IPublishedContent? GetContent(int? contentId = null)
        {
            using (UmbracoContextReference contextReference = this._umbracoContextFactory.EnsureUmbracoContext())
            {
                if (!contentId.HasValue)
                    contentId = contextReference.UmbracoContext.PublishedRequest?.PublishedContent?.Id;
                return contentId.HasValue ? this.GetContent(contextReference.UmbracoContext.Content, contentId.Value) : null;
            }
        }

        private IPublishedContent? GetContent(
          IPublishedContentCache? contentCache,
          int contentId)
        {
            return contentCache?.GetById(contentId);
        }

        private Hashtable GetPageElements(IPublishedContent? content)
        {
            Hashtable pageElements = new Hashtable();
            if (content != null)
            {
                pageElements.Add("pageID", content.Id);
                Hashtable hashtable = pageElements;
                IPublishedContent parent = content.Parent;
                var local = (ValueType)(parent != null ? parent.Id : -1);
                hashtable.Add("parentID", local);
                pageElements.Add("pageName", content.Name);
                pageElements.Add("nodeType", content.ContentType.Id);
                pageElements.Add("nodeTypeAlias", content.ContentType.Alias);
                pageElements.Add("writerName", content.WriterName());
                pageElements.Add("creatorName", content.CreatorName());
                pageElements.Add("createDate", content.CreateDate);
                pageElements.Add("updateDate", content.UpdateDate);
                pageElements.Add("path", content.Path);
                pageElements.Add("splitpath", content.Path.Split(new char[1]
                {
          ','
                }));
                int? templateId = content.TemplateId;
                if (templateId.HasValue)
                {
                    int valueOrDefault = templateId.GetValueOrDefault();
                    pageElements.Add("template", valueOrDefault);
                }
                foreach (IPublishedProperty property in content.Properties.Where<IPublishedProperty>(new Func<IPublishedProperty, bool>(this.IncludePropertyForPageElements)))
                {
                    if (!pageElements.ContainsKey(property.Alias))
                        pageElements.Add(property.Alias, property.Value(this._publishedValueFallback));
                }
            }
            return pageElements;
        }

        private bool IncludePropertyForPageElements(IPublishedProperty property) => !(new string[1]
        {
      "Umbraco.RichText"
        }).Contains<string>(property.PropertyType.EditorAlias);

        private bool TryGetValue(IPublishedProperty property, out object? value)
        {
            try
            {
                value = property.Value(this._publishedValueFallback);
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }
    }
}
