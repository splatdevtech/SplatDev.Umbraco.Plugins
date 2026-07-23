
// Type: Umbraco.Forms.Web.Api.DeliveryApi.FormsDeliveryApiControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Attributes;


#nullable enable
namespace Umbraco.Forms.Web.Api.DeliveryApi
{
  [ApiController]
  [UmbracoFormsDeliveryApiController]
  [MapToApi("forms-delivery")]
  public abstract class FormsDeliveryApiControllerBase : ControllerBase
  {
    protected FormsDeliveryApiControllerBase(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor)
    {
      this.FormService = formService;
      this.EntityService = entityService;
      this.PageService = pageService;
      this.VariationContextAccessor = variationContextAccessor;
    }

    protected IFormService FormService { get; }

    protected IEntityService EntityService { get; }

    public IPageService PageService { get; }

    public IVariationContextAccessor VariationContextAccessor { get; }

    protected bool TrySetRequestCulture(string culture)
    {
      CultureInfo cultureInfo;
      if (!FormsDeliveryApiControllerBase.TryGetCultureInfo(culture, out cultureInfo))
        return false;
      Thread.CurrentThread.CurrentCulture = cultureInfo;
      Thread.CurrentThread.CurrentUICulture = cultureInfo;
      this.VariationContextAccessor.VariationContext = new VariationContext(culture);
      return true;
    }

    private static bool TryGetCultureInfo(string culture, [NotNullWhen(true)] out CultureInfo? cultureInfo)
    {
      try
      {
        cultureInfo = new CultureInfo(culture);
        return true;
      }
      catch (CultureNotFoundException ex)
      {
        cultureInfo = (CultureInfo) null;
        return false;
      }
    }

    protected BadRequestObjectResult BadRequestForInvalidCulture(
      string culture)
    {
      return this.BadRequest((object) new ProblemDetailsBuilder().WithTitle("Unrecognized culture code").WithDetail("The provided value of '" + culture + "' for the 'culture' parameter was not recognized as a valid culture code.").Build());
    }

    protected Hashtable? GetContentPageElements(string? contentId)
    {
      if (string.IsNullOrEmpty(contentId))
        return (Hashtable) null;
      int result1;
      if (int.TryParse(contentId, out result1))
        return this.PageService.GetPageElements(result1);
      Guid result2;
      if (Guid.TryParse(contentId, out result2))
      {
        Umbraco.Cms.Core.Attempt<int> id = this.EntityService.GetId(result2, UmbracoObjectTypes.Document);
        if (id.Success)
          return this.PageService.GetPageElements(id.Result);
      }
      return (Hashtable) null;
    }
  }
}
