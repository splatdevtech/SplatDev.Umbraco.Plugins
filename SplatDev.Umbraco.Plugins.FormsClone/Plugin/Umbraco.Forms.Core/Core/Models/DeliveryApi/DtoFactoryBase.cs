
// Type: Umbraco.Forms.Core.Models.DeliveryApi.DtoFactoryBase
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public abstract class DtoFactoryBase
  {
    protected DtoFactoryBase(
      IEntityService entityService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder)
    {
      this.EntityService = entityService;
      this.PublishedContentCache = publishedContentCache;
      this.ApiContentRouteBuilder = apiContentRouteBuilder;
    }

    protected IEntityService EntityService { get; }

    protected IPublishedContentCache PublishedContentCache { get; }

    protected IApiContentRouteBuilder ApiContentRouteBuilder { get; }

    protected void PopulateGotoPageOnSubmit(Form form, IPostSubmissionDetail dto)
    {
      if (string.IsNullOrWhiteSpace(form.GoToPageOnSubmit) || form.GoToPageOnSubmit == "0")
        return;
      int result1;
      Guid result2;
      if (int.TryParse(form.GoToPageOnSubmit, out result1))
      {
        Umbraco.Cms.Core.Attempt<Guid> key = this.EntityService.GetKey(result1, UmbracoObjectTypes.Document);
        if (!key.Success)
          return;
        result2 = key.Result;
      }
      else if (!Guid.TryParse(form.GoToPageOnSubmit, out result2))
        return;
      dto.GotoPageOnSubmit = new Guid?(result2);
      IPublishedContent content = this.GetContent(result2);
      if (content == null)
        return;
      dto.GotoPageOnSubmitRoute = this.ApiContentRouteBuilder.Build(content);
    }

    private IPublishedContent? GetContent(Guid id)
    {
      IPublishedContent byId = this.PublishedContentCache.GetById(id);
      return byId == null || byId.ContentType.ItemType != PublishedItemType.Content ? (IPublishedContent) null : byId;
    }
  }
}
