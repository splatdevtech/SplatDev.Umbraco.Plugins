
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.UpdateRecordController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.CompilerServices;
using System.Text.Json;

using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
    [Authorize(Policy = "EditEntries")]
    public class UpdateRecordController : RecordControllerBase
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IFieldTypeStorage _fieldTypeStorage;

        public UpdateRecordController(
          IFormService formService,
          IRecordStorage recordStorage,
          IFormsSecurity formsSecurity,
          IPlaceholderParsingService placeholderParsingService,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          IFieldTypeStorage fieldTypeStorage)
          : base(formService, recordStorage, formsSecurity, placeholderParsingService)
        {
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._fieldTypeStorage = fieldTypeStorage;
        }

        [HttpPut("{recordId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public IActionResult Update(
          Guid formId,
          Guid recordId,
          IEnumerable<UpdateRecordField> fieldValues)
        {
            if (!this.FormsSecurity.CanCurrentUserEditEntries())
                return this.Forbid("Current user does not have permissions to edit entries via the back-office.");
            Umbraco.Forms.Core.Models.Form form = this.FormService.Get(formId);
            if (form == null)
                return this.NotFound();
            Umbraco.Forms.Core.Persistence.Dtos.Record recordByUniqueId = this.RecordStorage.GetRecordByUniqueId(recordId, form);
            if (recordByUniqueId == null)
                return this.NotFound();
            this.PopulateFormFieldValues(fieldValues, form, recordByUniqueId);
            UpdateRecordResult updateRecordResult = this.ValidateAndApplyUpdatedFieldValues(fieldValues, form, recordByUniqueId);
            if (updateRecordResult.Success)
            {
                this.RecordStorage.UpdateRecord(recordByUniqueId, form, this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id);
                return this.Ok();
            }
            ProblemDetailsBuilder problemDetailsBuilder = new ProblemDetailsBuilder();
            problemDetailsBuilder.WithRequestModelErrors(updateRecordResult.FieldMessages.ToDictionary<UpdateRecordResult.FieldMessage, string, string[]>(x => x.FieldId.ToString(), x => x.Messages.ToArray()));
            return this.BadRequest(problemDetailsBuilder.Build());
        }

        private void PopulateFormFieldValues(
          IEnumerable<UpdateRecordField> fieldValues,
          Umbraco.Forms.Core.Models.Form form,
          Umbraco.Forms.Core.Persistence.Dtos.Record record)
        {
            foreach (KeyValuePair<Guid, RecordField> recordField1 in record.RecordFields)
            {
                KeyValuePair<Guid, RecordField> recordField = recordField1;
                Field field = form.AllFields.SingleOrDefault<Field>(x => x.Id == recordField.Value.FieldId);
                if (field != null)
                {
                    UpdateRecordField updateRecordField = fieldValues.SingleOrDefault<UpdateRecordField>(x => x.FieldId == recordField.Value.FieldId);
                    field.Values = updateRecordField == null ? recordField.Value.Values : updateRecordField.Values.ToList<object>();
                }
            }
        }

        private UpdateRecordResult ValidateAndApplyUpdatedFieldValues(
          IEnumerable<UpdateRecordField> fieldValues,
          Umbraco.Forms.Core.Models.Form form,
          Umbraco.Forms.Core.Persistence.Dtos.Record record)
        {
            UpdateRecordResult updateRecordResult = new UpdateRecordResult()
            {
                Success = true
            };
            List<RecordField> list = record.RecordFields.Values.ToList<RecordField>();
            foreach (UpdateRecordField fieldValue1 in fieldValues)
            {
                UpdateRecordField fieldValue = fieldValue1;
                RecordField recordField = list.SingleOrDefault<RecordField>(x => x.FieldId == fieldValue.FieldId);
                if (recordField == null)
                {
                    Field field = form.AllFields.SingleOrDefault<Field>(x => x.Id == fieldValue.FieldId);
                    if (field == null)
                    {
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
                        interpolatedStringHandler.AppendLiteral("Field on form ");
                        interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
                        interpolatedStringHandler.AppendLiteral(" with id ");
                        interpolatedStringHandler.AppendFormatted<Guid>(fieldValue.FieldId);
                        interpolatedStringHandler.AppendLiteral(" not found.");
                        throw new ArgumentException(interpolatedStringHandler.ToStringAndClear(), "FieldId");
                    }
                    recordField = new RecordField(field);
                    record.RecordFields.Add(fieldValue.FieldId, recordField);
                }
                if (recordField.Field != null)
                {
                    Umbraco.Forms.Core.FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(recordField.Field);
                    if (fieldTypeByField != null)
                    {
                        IList<string> source = fieldTypeByField.ValidateField(form, recordField.Field, fieldValue.Values, this.HttpContext, this.PlaceholderParsingService, this._fieldTypeStorage).ToList<string>() ?? new List<string>();
                        if (source.Count == 0)
                        {
                            recordField.Values = fieldValue.Values.Select<object, object>(new Func<object, object>(this.ToRecordValue)).ToList<object>();
                        }
                        else
                        {
                            updateRecordResult.Success = false;
                            updateRecordResult.FieldMessages.Add(new UpdateRecordResult.FieldMessage()
                            {
                                FieldId = fieldValue.FieldId,
                                Messages = source.Select<string, string>(x => this.PlaceholderParsingService.ParsePlaceholdersForValidationErrorMessage(form, recordField.Field, x)).ToList<string>()
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
                        return (object)jsonElement.GetString() ?? string.Empty;
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
