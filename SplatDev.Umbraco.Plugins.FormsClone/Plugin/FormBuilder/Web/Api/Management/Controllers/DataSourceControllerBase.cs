using FormBuilder.Core.DataSources;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with datasources.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Data Source")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageDataSources")]
    [Route("/formBuilder/management/api/v1/data-source")]
    public abstract class DataSourceControllerBase(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection formDataSourcesType) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IDataSourceService DataSourceService { get; } = dataSourceService;

        /// <summary>
        /// Gets the         /// </summary>
        protected DataSourceTypeCollection DataSourcesTypeCollection { get; } = formDataSourcesType;

        /// <summary>Validates a prevalue source.</summary>
        protected ProviderValidationResult TryValidateProviderType(
          FormDataSource dataSource,
          out ProblemDetails? problemDetails)
        {
            problemDetails = null;
            FormDataSourceType? formDataSourceType = dataSource.GetFormDataSourceType(DataSourcesTypeCollection);
            if (formDataSourceType is null)
                return ProviderValidationResult.FailedTypeNotFound;
            formDataSourceType.LoadSettings(dataSource);
            List<Exception> exceptionList = formDataSourceType.ValidateSettings();
            if (exceptionList.Count == 0)
                return ProviderValidationResult.Success;
            problemDetails = BuildSettingsValidationProblemDetails(exceptionList);
            return ProviderValidationResult.FailedValidation;
        }
    }
}