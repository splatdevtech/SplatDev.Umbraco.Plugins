using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for validating the settings for a field.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class ValidateFieldSettingsController(IFieldTypeStorage fieldTypeStorage) : FormFieldControllerBase(fieldTypeStorage)
    {
        /// <summary>
        /// Management API endpoint for validating the settings for a field.
        /// </summary>
        [HttpPost("{id:guid}/validate-settings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult ValidateSettings(Guid id, ValidateFieldSettingsModel model)
        {
            FieldType? fieldTypeByField = FieldTypeStorage.GetFieldTypeByField(id, model.Settings);
            if (fieldTypeByField is null)
                return BadRequest(BuildSettingsValidationProblemDetails(new Exception("Field type not found.")));
            List<Exception> exceptionList = fieldTypeByField.ValidateSettings();
            exceptionList.AddRange(ValidateFileUploadSettings(model, fieldTypeByField));
            if (string.IsNullOrWhiteSpace(model.Caption) || string.IsNullOrWhiteSpace(model.Alias))
                exceptionList.Add(new Exception("A caption and alias for the field must be provided."));
            if (model.Alias.Length > byte.MaxValue)
                exceptionList.Add(new Exception("The field alias cannot be longer than 255 characters."));
            return exceptionList.Count != 0 ? BadRequest(FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList)) : Ok();
        }

        private static List<Exception> ValidateFileUploadSettings(
          ValidateFieldSettingsModel model,
          FieldType fieldType)
        {
            List<Exception> exceptionList = [];
            if (!fieldType.SupportsUploadTypes)
                return exceptionList;
            IEnumerable<AllowedUploadType>? allowedUploadTypes = model.AllowedUploadTypes;
            List<AllowedUploadType> source = (allowedUploadTypes?.ToList()) ?? [];
            if (!source[0].Checked.Equals("false", StringComparison.OrdinalIgnoreCase) || source.Skip(1).Any(x => string.IsNullOrEmpty(x.Checked) || x.Checked.Equals("true", StringComparison.InvariantCultureIgnoreCase)))
                return exceptionList;
            exceptionList.Add(new Exception("File uploads are restricted to specified file types only, but no file types have been allowed."));
            return exceptionList;
        }
    }
}