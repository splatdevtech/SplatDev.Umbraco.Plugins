
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType.PrevalueSourceTypeControllerBase
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
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSourceType
{
  [ApiExplorerSettings(GroupName = "Prevalue Source Type")]
  [Authorize(Policy = "SectionAccessForms")]
  [Route("/umbraco/forms/management/api/v1/prevalue-source-type")]
  public abstract class PrevalueSourceTypeControllerBase : FormsManagementApiControllerBase
  {
    protected PrevalueSourceTypeControllerBase(
      FieldPreValueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
    {
      this.PrevalueSourceTypeCollection = prevalueSourceTypeCollection;
      this.HostingEnvironment = hostingEnvironment;
      this.FormDesignSettings = formDesignSettings.Value;
    }

    protected FieldPreValueSourceCollection PrevalueSourceTypeCollection { get; }

    protected IHostingEnvironment HostingEnvironment { get; }

    protected FormDesignSettings FormDesignSettings { get; }

    protected PreValueSourceTypeWithSettings CreatePrevalueSourceTypeWithSettings(
      FieldPreValueSourceType type)
    {
      PreValueSourceTypeWithSettings providerType = new PreValueSourceTypeWithSettings();
      providerType.Id = type.Id;
      providerType.Alias = type.Alias;
      providerType.Name = type.Name;
      providerType.Description = type.Description;
      providerType.Icon = type.Icon;
      providerType.ApplySettings(type.Settings(), this.HostingEnvironment, this.FormDesignSettings.SettingsCustomization.PrevalueSourceTypes.GetValueForProviderType((ProviderBase) type));
      return providerType;
    }
  }
}
