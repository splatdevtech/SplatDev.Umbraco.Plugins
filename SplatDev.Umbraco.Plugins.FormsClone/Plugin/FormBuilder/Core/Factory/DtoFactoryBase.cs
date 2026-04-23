using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;

using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Factory
{
    public abstract class DtoFactoryBase(
      IEntityService entityService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder)
    {
        protected IEntityService EntityService { get; } = entityService;

        protected IPublishedContentCache PublishedContentCache { get; } = publishedContentCache;

        protected IApiContentRouteBuilder ApiContentRouteBuilder { get; } = apiContentRouteBuilder;

        protected void PopulateGotoPageOnSubmit(Form? form, IPostSubmissionDetail dto)
        {
            if (form is null || string.IsNullOrWhiteSpace(form.GoToPageOnSubmit) || form.GoToPageOnSubmit == "0")
                return;
            Guid result2;
            if (int.TryParse(form.GoToPageOnSubmit, out int result1))
            {
                Umbraco.Cms.Core.Attempt<Guid> key = EntityService.GetKey(result1, UmbracoObjectTypes.Document);
                if (!key.Success)
                    return;
                result2 = key.Result;
            }
            else if (!Guid.TryParse(form.GoToPageOnSubmit, out result2))
                return;
            dto.GotoPageOnSubmit = new Guid?(result2);
            IPublishedContent? content = GetContent(result2);
            if (content is null)
                return;
            dto.GotoPageOnSubmitRoute = ApiContentRouteBuilder.Build(content);
        }

        private IPublishedContent? GetContent(Guid id)
        {
            IPublishedContent? byId = PublishedContentCache.GetById(id);
            return byId is null || byId.ContentType.ItemType != PublishedItemType.Content ? null : byId;
        }
    }
}