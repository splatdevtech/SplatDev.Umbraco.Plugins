
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSource.DataSourceControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSource
{
  [ApiExplorerSettings(GroupName = "Data Source")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManageDataSources")]
  [Route("/umbraco/forms/management/api/v1/data-source")]
  public abstract class DataSourceControllerBase : FormsManagementApiControllerBase
  {
    protected DataSourceControllerBase(
      IDataSourceService dataSourceService,
      DataSourceTypeCollection formDataSourcesType)
    {
      this.DataSourcesTypeCollection = formDataSourcesType;
      this.DataSourceService = dataSourceService;
    }

    protected IDataSourceService DataSourceService { get; }

    protected DataSourceTypeCollection DataSourcesTypeCollection { get; }

    protected ProviderValidationResult TryValidateProviderType(
      FormDataSource dataSource,
      out ProblemDetails? problemDetails)
    {
      problemDetails = (ProblemDetails) null;
      FormDataSourceType formDataSourceType = dataSource.GetFormDataSourceType(this.DataSourcesTypeCollection);
      if (formDataSourceType == null)
        return ProviderValidationResult.FailedTypeNotFound;
      formDataSourceType.LoadSettings(dataSource);
      List<Exception> exceptionList = formDataSourceType.ValidateSettings();
      if (!exceptionList.Any<Exception>())
        return ProviderValidationResult.Success;
      problemDetails = FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList);
      return ProviderValidationResult.FailedValidation;
    }
  }
}
