using FormBuilder.Core.Configuration;
using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with data source types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Data Source Type")]
    [Route("/formBuilder/management/api/v1/data-source-type")]
    [Authorize(Policy = "SectionAccessForms")]
    public abstract class DataSourceTypeControllerBase(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected DataSourceTypeCollection DataSourceTypeCollection { get; } = dataSourceTypeCollection;

        /// <summary>
        /// Gets the         /// </summary>
        protected IHostingEnvironment HostingEnvironment { get; } = hostingEnvironment;

        /// <summary>
        /// Gets the         /// </summary>
        protected FormDesignSettings FormDesignSettings { get; } = formDesignSettings.Value;

        /// <summary>
        /// Creates a         /// </summary>
        protected DataSourceTypeWithSettings CreateDataSourceTypeWithSettings(
          FormDataSourceType type)
        {
            DataSourceTypeWithSettings providerType = new()
            {
                Id = type.Id,
                Alias = type.Alias,
                Name = type.Name,
                Description = type.Description,
                Icon = type.Icon
            };
            providerType.ApplySettings(type.Settings(), FormDesignSettings.SettingsCustomization.DataSourceTypes.GetValueForProviderType(type));
            return providerType;
        }
    }
}