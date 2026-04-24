
// Type: Umbraco.Forms.Web.Api.ManagementApi.DataSourceType.DataSourceTypeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.DataSourceType
{
  [ApiExplorerSettings(GroupName = "Data Source Type")]
  [Route("/umbraco/forms/management/api/v1/data-source-type")]
  [Authorize(Policy = "SectionAccessForms")]
  public abstract class DataSourceTypeControllerBase : FormsManagementApiControllerBase
  {
    protected DataSourceTypeControllerBase(
      DataSourceTypeCollection dataSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
    {
      this.DataSourceTypeCollection = dataSourceTypeCollection;
      this.HostingEnvironment = hostingEnvironment;
      this.FormDesignSettings = formDesignSettings.Value;
    }

    protected DataSourceTypeCollection DataSourceTypeCollection { get; }

    protected IHostingEnvironment HostingEnvironment { get; }

    protected FormDesignSettings FormDesignSettings { get; }

    protected DataSourceTypeWithSettings CreateDataSourceTypeWithSettings(
      FormDataSourceType type)
    {
      DataSourceTypeWithSettings providerType = new DataSourceTypeWithSettings();
      providerType.Id = type.Id;
      providerType.Alias = type.Alias;
      providerType.Name = type.Name;
      providerType.Description = type.Description;
      providerType.Icon = type.Icon;
      providerType.ApplySettings(type.Settings(), this.HostingEnvironment, this.FormDesignSettings.SettingsCustomization.DataSourceTypes.GetValueForProviderType((ProviderBase) type));
      return providerType;
    }
  }
}
