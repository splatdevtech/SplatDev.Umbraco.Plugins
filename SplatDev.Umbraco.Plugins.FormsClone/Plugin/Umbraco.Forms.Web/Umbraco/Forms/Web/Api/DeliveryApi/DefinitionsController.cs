
// Type: Umbraco.Forms.Web.Api.DeliveryApi.DefinitionsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Attributes;


#nullable enable
namespace Umbraco.Forms.Web.Api.DeliveryApi
{
  [Route("/umbraco/forms/delivery/api/v1/definitions")]
  public class DefinitionsController : FormsDeliveryApiControllerBase
  {
    private readonly FormDtoFactory _formDtoFactory;

    public DefinitionsController(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor,
      FormDtoFactory formDtoFactory)
      : base(formService, entityService, pageService, variationContextAccessor)
    {
      this._formDtoFactory = formDtoFactory;
    }

    [HttpGet("{id:guid}")]
    [ApiExplorerSettings(GroupName = "Forms")]
    [ValidateFormsApiIsEnabled]
    [ValidateFormsApiKey]
    [ProducesResponseType(typeof (FormDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [SwaggerParameter("id", "The form's Id.")]
    [SwaggerParameter("contentId", "The Id of the content page on which the form is hosted.")]
    [SwaggerParameter("culture", "The culture code for the form's localization context.")]
    [SwaggerParameter("additionalData", "Additional data provided when rendering the form.")]
    public ActionResult<FormDto> GetById(
      Guid id,
      string? contentId = null,
      string? culture = null,
      [FromQuery] IDictionary<string, string?>? additionalData = null)
    {
      if (!string.IsNullOrWhiteSpace(culture) && !this.TrySetRequestCulture(culture))
        return (ActionResult<FormDto>) (ActionResult) this.BadRequestForInvalidCulture(culture);
      Form form = this.FormService.Get(id);
      if (form == null)
        return (ActionResult<FormDto>) (ActionResult) this.NotFound();
      Hashtable contentPageElements = this.GetContentPageElements(contentId);
      return (ActionResult<FormDto>) (ActionResult) this.Ok((object) this._formDtoFactory.BuildFormDefinitionDto(form, contentPageElements, additionalData));
    }
  }
}
