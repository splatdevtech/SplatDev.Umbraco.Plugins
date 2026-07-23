using FormBuilder.Core.Dto;
using FormBuilder.Core.Factory;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Attributes;

using Microsoft.AspNetCore.Mvc;

using System.Collections;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Defines Umbraco Forms API operations for retrieving form definitions.
    /// </summary>
    /// <remarks>
    /// Instantiates a new instance of the     /// </remarks>
    [Route("/formbuilder/delivery/api/v1/definitions")]
    public class DefinitionsController(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor,
      FormDtoFactory formDtoFactory) : FormsDeliveryApiControllerBase(formService, entityService, pageService, variationContextAccessor)
    {
        private readonly FormDtoFactory _formDtoFactory = formDtoFactory;

        /// <summary>Retrieves a single form by Id.</summary>
        [HttpGet("{id:guid}")]
        [ApiExplorerSettings(GroupName = "Forms")]
        [ValidateFormsApiIsEnabled]
        [ValidateFormsApiKey]
        [ProducesResponseType(typeof(FormDto), 200)]
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
            if (!string.IsNullOrWhiteSpace(culture) && !TrySetRequestCulture(culture))
                return (ActionResult<FormDto>)BadRequestForInvalidCulture(culture);
            Core.Models.Form? form = FormService.Get(id);
            if (form is null)
                return (ActionResult<FormDto>)NotFound();
            Hashtable? contentPageElements = GetContentPageElements(contentId);
            return (ActionResult<FormDto>)Ok(_formDtoFactory.BuildFormDefinitionDto(form, contentPageElements, additionalData));
        }
    }
}