
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Field.ValidateFieldSettingsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;

using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Field
{
    public class ValidateFieldSettingsController : FormFieldControllerBase
    {
        public ValidateFieldSettingsController(IFieldTypeStorage fieldTypeStorage)
          : base(fieldTypeStorage)
        {
        }

        [HttpPost("{id:guid}/validate-settings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult ValidateSettings(Guid id, ValidateFieldSettingsModel model)
        {
            Umbraco.Forms.Core.FieldType fieldTypeByField = this.FieldTypeStorage.GetFieldTypeByField(id, model.Settings);
            if (fieldTypeByField == null)
                return this.BadRequest(FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(new Exception("Field type not found.")));
            List<Exception> exceptionList = fieldTypeByField.ValidateSettings();
            exceptionList.AddRange(ValidateFieldSettingsController.ValidateFileUploadSettings(model, fieldTypeByField));
            if (string.IsNullOrWhiteSpace(model.Caption) || string.IsNullOrWhiteSpace(model.Alias))
                exceptionList.Add(new Exception("A caption and alias for the field must be provided."));
            if (model.Alias.Length > byte.MaxValue)
                exceptionList.Add(new Exception("The field alias cannot be longer than 255 characters."));
            return exceptionList.Any<Exception>() ? this.BadRequest(FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList)) : this.Ok();
        }

        private static List<Exception> ValidateFileUploadSettings(
          ValidateFieldSettingsModel model,
          Umbraco.Forms.Core.FieldType fieldType)
        {
            List<Exception> exceptionList = new List<Exception>();
            if (!fieldType.SupportsUploadTypes)
                return exceptionList;
            IEnumerable<AllowedUploadType> allowedUploadTypes = model.AllowedUploadTypes;
            List<AllowedUploadType> source = (allowedUploadTypes != null ? allowedUploadTypes.ToList<AllowedUploadType>() : null) ?? new List<AllowedUploadType>();
            if (!(source[0].Checked.ToLowerInvariant() == "false") || source.Skip<AllowedUploadType>(1).Any<AllowedUploadType>(x => string.IsNullOrEmpty(x.Checked) || x.Checked.ToLowerInvariant() == "true"))
                return exceptionList;
            exceptionList.Add(new Exception("File uploads are restricted to specified file types only, but no file types have been allowed."));
            return exceptionList;
        }
    }
}
