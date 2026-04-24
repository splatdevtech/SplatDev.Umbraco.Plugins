using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Attributes;

using Microsoft.AspNetCore.Mvc;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Defines a base controller for the Umbraco Forms API.</summary>
    /// <remarks>
    /// Note that we aren't inheriting from UmbracoApiController here.
    /// Reason is that for the endpoint to be picked up by Swagger we need to define a route, and so we can't use Umbraco's auto-routing.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiController]
    [FormBuilderDeliveryApiController]
    [MapToApi("forms-delivery")]
    public abstract class FormsDeliveryApiControllerBase(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor) : ControllerBase
    {
        /// <summary>Gets the form service.</summary>
        protected IFormService FormService { get; } = formService;

        /// <summary>Gets the entity service.</summary>
        protected IEntityService EntityService { get; } = entityService;

        /// <summary>Gets the page service.</summary>
        public IPageService PageService { get; } = pageService;

        /// <summary>Gets the page service.</summary>
        public IVariationContextAccessor VariationContextAccessor { get; } = variationContextAccessor;

        /// <summary>
        /// Sets the request thread's culture based on the provided parameter.
        /// </summary>
        /// <param name="culture">The culture code.</param>
        /// <returns>Boolean result indicating whether the provided culture code could be parsed as a CultureInfo and used to set the request culture.</returns>
        protected bool TrySetRequestCulture(string culture)
        {
            if (!TryGetCultureInfo(culture, out CultureInfo? cultureInfo))
                return false;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            VariationContextAccessor.VariationContext = new VariationContext(culture);
            return true;
        }

        private static bool TryGetCultureInfo(string culture, [NotNullWhen(true)] out CultureInfo? cultureInfo)
        {
            try
            {
                cultureInfo = new CultureInfo(culture);
                return true;
            }
            catch (CultureNotFoundException)
            {
                cultureInfo = null;
                return false;
            }
        }

        /// <summary>
        /// Generates a         /// </summary>
        /// <param name="culture">The provided culture code.</param>
        protected BadRequestObjectResult BadRequestForInvalidCulture(
          string culture)
        {
            return BadRequest(new ProblemDetailsBuilder().WithTitle("Unrecognized culture code").WithDetail("The provided value of '" + culture + "' for the 'culture' parameter was not recognized as a valid culture code.").Build());
        }

        /// <summary>Gets the page elements for the provided content Id,</summary>
        /// <param name="contentId">The content Id (provided as an integer or Guid).</param>
        /// <returns></returns>
        protected Hashtable? GetContentPageElements(string? contentId)
        {
            if (string.IsNullOrEmpty(contentId))
                return null;
            if (int.TryParse(contentId, out int result1))
                return PageService.GetPageElements(result1);
            if (Guid.TryParse(contentId, out Guid result2))
            {
                Umbraco.Cms.Core.Attempt<int> id = EntityService.GetId(result2, UmbracoObjectTypes.Document);
                if (id.Success)
                    return PageService.GetPageElements(id.Result);
            }
            return null;
        }
    }
}