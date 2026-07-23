using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with prevalue source types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Prevalue Source Type")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/prevalue-source-type")]
    public abstract class PrevalueSourceTypeControllerBase(
      FieldPrevalueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected FieldPrevalueSourceCollection PrevalueSourceTypeCollection { get; } = prevalueSourceTypeCollection;

        /// <summary>
        /// Gets the         /// </summary>
        protected IHostingEnvironment HostingEnvironment { get; } = hostingEnvironment;

        /// <summary>
        /// Gets the         /// </summary>
        protected FormDesignSettings FormDesignSettings { get; } = formDesignSettings.Value;

        /// <summary>
        /// Creates a         /// </summary>
        protected PrevalueSourceTypeWithSettings CreatePrevalueSourceTypeWithSettings(
          FieldPrevalueSourceType type)
        {
            PrevalueSourceTypeWithSettings providerType = new()
            {
                Id = type.Id,
                Alias = type.Alias,
                Name = type.Name,
                Description = type.Description,
                Icon = type.Icon
            };
            providerType.ApplySettings(type.Settings(), FormDesignSettings.SettingsCustomization.PrevalueSourceTypes.GetValueForProviderType(type));
            return providerType;
        }
    }
}