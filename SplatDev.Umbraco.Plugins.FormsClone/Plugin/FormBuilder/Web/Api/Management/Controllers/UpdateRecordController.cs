using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Extensions;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.CompilerServices;
using System.Text.Json;

using Umbraco.Cms.Api.Common.Builders;

using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for updating a record.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "EditEntries")]
    public class UpdateRecordController(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFieldTypeStorage fieldTypeStorage) : RecordControllerBase(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;

        /// <summary>Management API endpoint for updating a record.</summary>
        [HttpPut("{recordId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult Update(
          Guid formId,
          Guid recordId,
          IEnumerable<UpdateRecordField> fieldValues)
        {
            if (!FormsSecurity.CanCurrentUserEditEntries())
                return Forbid("Current user does not have permissions to edit entries via the back-office.");
            Form? form = FormService.Get(formId);
            if (form is null)
                return NotFound();
            Record? recordByUniqueId = RecordStorage.GetRecordByUniqueId(recordId, form);
            if (recordByUniqueId is null)
                return NotFound();
            PopulateFormFieldValues(fieldValues, form, recordByUniqueId);
            UpdateRecordResult updateRecordResult = ValidateAndApplyUpdatedFieldValues(fieldValues, form, recordByUniqueId);
            if (updateRecordResult.Success)
            {
                RecordStorage.UpdateRecord(recordByUniqueId, form, _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id);
                return Ok();
            }
            ProblemDetailsBuilder problemDetailsBuilder = new();
            problemDetailsBuilder.WithRequestModelErrors(updateRecordResult.FieldMessages.ToDictionary<UpdateRecordResult.FieldMessage, string, string[]>(x => x.FieldId.ToString(), x => [.. x.Messages]));
            return BadRequest(problemDetailsBuilder.Build());
        }

        private static void PopulateFormFieldValues(
          IEnumerable<UpdateRecordField> fieldValues,
          Form form,
          Record record)
        {
            foreach (KeyValuePair<Guid, RecordField> recordField1 in record.RecordFields)
            {
                KeyValuePair<Guid, RecordField> recordField = recordField1;
                var field = form.AllFields.SingleOrDefault(x => x.Id == recordField.Value.FieldId);
                if (field is not null)
                {
                    UpdateRecordField? updateRecordField = fieldValues.SingleOrDefault(x => x.FieldId == recordField.Value.FieldId);
                    field.Values = updateRecordField is null ? recordField.Value.Values : [.. updateRecordField.Values];
                }
            }
        }

        private UpdateRecordResult ValidateAndApplyUpdatedFieldValues(
          IEnumerable<UpdateRecordField> fieldValues,
          Form form,
          Record record)
        {
            UpdateRecordResult updateRecordResult = new()
            {
                Success = true
            };
            List<RecordField> list = [.. record.RecordFields.Values];
            foreach (UpdateRecordField fieldValue1 in fieldValues)
            {
                UpdateRecordField fieldValue = fieldValue1;
                RecordField? recordField = list.SingleOrDefault(x => x.FieldId == fieldValue.FieldId);
                if (recordField is null)
                {
                    var field = form.AllFields.SingleOrDefault(x => x.Id == fieldValue.FieldId);
                    if (field is null)
                    {
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new(34, 2);
                        interpolatedStringHandler.AppendLiteral("Field on form ");
                        interpolatedStringHandler.AppendFormatted(form.Id);
                        interpolatedStringHandler.AppendLiteral(" with id ");
                        interpolatedStringHandler.AppendFormatted(fieldValue.FieldId);
                        interpolatedStringHandler.AppendLiteral(" not found.");
                        throw new ArgumentException(interpolatedStringHandler.ToStringAndClear(), nameof(form));
                    }
                    recordField = new RecordField(field);
                    record.RecordFields.Add(fieldValue.FieldId, recordField);
                }
                if (recordField.Field is not null)
                {
                    FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                    if (fieldTypeByField is not null)
                    {
                        if (((IList<string>)(fieldTypeByField.ValidateField(form, recordField.Field, fieldValue.Values, HttpContext, PlaceholderParsingService, _fieldTypeStorage).ToList() ?? [])).Count == 0)
                        {
                            recordField.Values = [.. fieldValue.Values.Select(new Func<object, object>(ToRecordValue))];
                        }
                        else
                        {
                            updateRecordResult.Success = false;
                            updateRecordResult.FieldMessages.Add(new UpdateRecordResult.FieldMessage()
                            {
                                FieldId = fieldValue.FieldId,
                                Messages = [.. (fieldTypeByField.ValidateField(form, recordField.Field, fieldValue.Values, HttpContext, PlaceholderParsingService, _fieldTypeStorage).ToList() ?? []).Select(x => PlaceholderParsingService.ParsePlaceholdersForValidationErrorMessage(form, recordField.Field, x))]
                            });
                        }
                    }
                }
            }
            return updateRecordResult;
        }

        private object ToRecordValue(object inputValue)
        {
            if (inputValue is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                    case JsonValueKind.Null:
                        return string.Empty;

                    case JsonValueKind.String:
                        return jsonElement.GetString() as object ?? string.Empty;

                    case JsonValueKind.Number:
                        return jsonElement.GetInt32();

                    case JsonValueKind.True:
                        return true;

                    case JsonValueKind.False:
                        return false;
                }
            }
            return inputValue;
        }
    }
}