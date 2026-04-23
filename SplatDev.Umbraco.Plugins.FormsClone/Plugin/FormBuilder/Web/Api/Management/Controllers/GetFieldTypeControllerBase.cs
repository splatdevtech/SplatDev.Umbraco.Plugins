using FormBuilder.Core.Configuration;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with retrieving field types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public abstract class GetFieldTypeControllerBase(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection) : FieldTypeControllerBase(fieldTypeStorage)
    {
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;

        /// <summary>
        /// Gets the         /// </summary>
        protected FieldCollection FieldCollection { get; } = fieldCollection;

        /// <summary>
        /// Creates a field type model from the         /// </summary>
        protected FieldTypeWithSettings CreateFieldTypeWithSettings(
          FieldType fieldType)
        {
            FieldTypeWithSettings providerType = new()
            {
                Id = fieldType.Id,
                Alias = fieldType.Alias,
                Name = fieldType.Name,
                Description = fieldType.Description,
                Icon = fieldType.Icon,
                Group = fieldType.Category,
                RenderInputType = fieldType.RenderInputType,
                SortOrder = fieldType.SortOrder,
                SupportsPrevalues = fieldType.SupportsPreValues,
                SupportsUploadTypes = fieldType.SupportsUploadTypes,
                SupportsMandatory = fieldType.SupportsMandatory,
                MandatoryByDefault = fieldType.MandatoryByDefault,
                SupportsRegex = fieldType.SupportsRegex,
                HideLabel = fieldType.HideLabel,
                View = _hostingEnvironment.ToAbsolute(fieldType.GetDesignView()),
                PreviewView = fieldType.PreviewView
            };
            providerType.ApplySettings(fieldType.Settings(), _formDesignSettings.SettingsCustomization.FieldTypes.GetValueForProviderType(fieldType));
            return providerType;
        }
    }
}