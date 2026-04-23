
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Plugins.CacheManager.Models
{
    public abstract class Configuration : IPublishedContent
    {
        public int Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string? UrlSegment => throw new NotImplementedException();

        public int SortOrder => throw new NotImplementedException();

        public int Level => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public int? TemplateId => throw new NotImplementedException();

        public int CreatorId => throw new NotImplementedException();

        public DateTime CreateDate => throw new NotImplementedException();

        public int WriterId => throw new NotImplementedException();

        public DateTime UpdateDate => throw new NotImplementedException();

        public IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        public PublishedItemType ItemType => throw new NotImplementedException();

        public IPublishedContent? Parent => throw new NotImplementedException();

        public IEnumerable<IPublishedContent> Children => throw new NotImplementedException();

        public IEnumerable<IPublishedContent> ChildrenForAllCultures => throw new NotImplementedException();

        public IPublishedContentType ContentType => throw new NotImplementedException();

        public Guid Key => throw new NotImplementedException();

        public IEnumerable<IPublishedProperty> Properties => throw new NotImplementedException();

        public IPublishedProperty? GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public bool IsDraft(string? culture = null)
        {
            throw new NotImplementedException();
        }

        public bool IsPublished(string? culture = null)
        {
            throw new NotImplementedException();
        }
    }
}
