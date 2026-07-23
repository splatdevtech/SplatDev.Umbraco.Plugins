
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.GetFieldTypeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  public abstract class GetFieldTypeControllerBase : FieldTypeControllerBase
  {
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly FormDesignSettings _formDesignSettings;

    protected GetFieldTypeControllerBase(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection)
      : base(fieldTypeStorage)
    {
      this._hostingEnvironment = hostingEnvironment;
      this._formDesignSettings = formDesignSettings.Value;
      this.FieldCollection = fieldCollection;
    }

    protected FieldCollection FieldCollection { get; }

    protected FieldTypeWithSettings CreateFieldTypeWithSettings(
      Umbraco.Forms.Core.FieldType fieldType)
    {
      FieldTypeWithSettings providerType = new FieldTypeWithSettings();
      providerType.Id = fieldType.Id;
      providerType.Alias = fieldType.Alias;
      providerType.Name = fieldType.Name;
      providerType.Description = fieldType.Description;
      providerType.Icon = fieldType.Icon;
      providerType.Group = fieldType.Category;
      providerType.RenderInputType = fieldType.RenderInputType;
      providerType.SortOrder = fieldType.SortOrder;
      providerType.SupportsPrevalues = fieldType.SupportsPreValues;
      providerType.SupportsUploadTypes = fieldType.SupportsUploadTypes;
      providerType.SupportsMandatory = fieldType.SupportsMandatory;
      providerType.MandatoryByDefault = fieldType.MandatoryByDefault;
      providerType.SupportsRegex = fieldType.SupportsRegex;
      providerType.HideLabel = fieldType.HideLabel;
      providerType.View = this._hostingEnvironment.ToAbsolute(fieldType.GetDesignView());
      providerType.PreviewView = fieldType.PreviewView;
      providerType.ApplySettings(fieldType.Settings(), this._hostingEnvironment, this._formDesignSettings.SettingsCustomization.FieldTypes.GetValueForProviderType((ProviderBase) fieldType));
      return providerType;
    }
  }
}
