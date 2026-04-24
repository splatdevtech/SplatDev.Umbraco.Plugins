using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Dto;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Factory;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.ModelBinders;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Attributes;
using FormBuilder.Web.Helpers;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System.Collections;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Defines Umbraco Forms API operations for submitting form definitions.
    /// </summary>
    /// <remarks>
    /// Instantiates a new instance of the     /// </remarks>
    [Route("/formbuilder/delivery/api/v1/entries")]
    public class EntriesController(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor,
      IMemberManager memberManager,
      IFieldTypeStorage fieldTypeStorage,
      IFieldPrevalueSourceService fieldPreValueSourceService,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      IPlaceholderParsingService placeholderParsingService,
      IRecordService recordService,
      IHostEnvironment hostEnvironment,
      IDataProtectionProvider dataProtectionProvider,
      IJsonSerializer jsonSerializer,
      IOptions<SecuritySettings> securitySettings,
      IOptions<PackageOptionSettings> packageOptionSettings,
      IEventMessagesFactory eventMessagesFactory,
      IEventAggregator eventAggregator,
      EntryAcceptedDtoFactory entryAcceptedDtoFactory) : FormsDeliveryApiControllerBase(formService, entityService, pageService, variationContextAccessor)
    {
        private readonly IMemberManager _memberManager = memberManager;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IFieldPrevalueSourceService _fieldPreValueSourceService = fieldPreValueSourceService;
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;
        private readonly IRecordService _recordService = recordService;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider;
        private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly EntryAcceptedDtoFactory _entryAcceptedDtoFactory = entryAcceptedDtoFactory;
        private IDataProtector? _dataProtector;

        /// <summary>Gets the data protector.</summary>
        /// <value>The data protector.</value>
        /// <remarks>
        /// Must match name used in FormBuilder.Core.Providers.FieldTypes.FileUpload.
        /// </remarks>
        private IDataProtector DataProtector => _dataProtector ??= _dataProtectionProvider.CreateProtector("FormBuilder.FileUpload");

        /// <summary>Processes a submission for a form.</summary>
        [HttpPost("{id:guid}")]
        [ApiExplorerSettings(GroupName = "Forms")]
        [IgnoreAntiforgeryToken]
        [ValidateFormsApiIsEnabled]
        [ValidateFormsApiKey]
        [ValidateFormsApiAntiForgeryToken]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [SwaggerParameter("id", "The form's Id.")]
        public async Task<ActionResult> SubmitEntry(Guid id, [FromBody, ModelBinder(typeof(FormBuilderApiFormEntryDtoModelBinder))] FormEntryDto entry)
        {
            EntriesController entriesController = this;
            if (!string.IsNullOrWhiteSpace(entry.Culture) && !entriesController.TrySetRequestCulture(entry.Culture))
                return entriesController.BadRequestForInvalidCulture(entry.Culture);
            if (entriesController.FormService.Get(id) is null)
                return entriesController.NotFound();
            Record record = new();
            FormSubmissionHelper.InitializeRecord(record, entriesController.FormService.Get(id)!, entriesController.HttpContext, entriesController._packageOptionSettings.EnableRecordingOfIpWithFormSubmission, entry.AdditionalData);
            entriesController.ApplyContentId(entry, record);
            await FormSubmissionHelper.ApplyMemberKey(record, entriesController.HttpContext, entriesController._memberManager);
            entriesController.ValidateAndPopulateRecordFields(entry, entriesController.FormService.Get(id), record);
            if (entriesController.ModelState.IsValid)
            {
                await entriesController._recordService.SubmitAsync(record, entriesController.FormService.Get(id));
                Hashtable? contentPageElements = entriesController.GetContentPageElements(entry.ContentId);
                EntryAcceptedDto entryAcceptedDto = entriesController._entryAcceptedDtoFactory.BuildEntryAcceptedDto(entriesController.FormService.Get(id), record, contentPageElements, entry.AdditionalData);
                return entriesController.Accepted(entryAcceptedDto);
            }
            ValidationProblemDetails problemDetails = new(entriesController.ModelState);
            entriesController.ParsePlaceholdersInValidationMessages(problemDetails, entriesController.FormService.Get(id), entry.ContentId, entry.AdditionalData);
            ObjectResult objectResult = new(problemDetails);
            objectResult.ContentTypes.Add("application/problem+json");
            objectResult.StatusCode = new int?(422);
            return objectResult;
        }

        private void ApplyContentId(FormEntryDto entry, Record record)
        {
            if (string.IsNullOrEmpty(entry.ContentId))
                return;
            if (int.TryParse(entry.ContentId, out int result1))
            {
                record.UmbracoPageId = result1;
            }
            else
            {
                if (Guid.TryParse(entry.ContentId, out Guid result2))
                {
                    Umbraco.Cms.Core.Attempt<int> id = EntityService.GetId(result2, UmbracoObjectTypes.Document);
                    if (id.Success)
                    {
                        record.UmbracoPageId = id.Result;
                        return;
                    }
                }
                ModelState.AddModelError("contentKey", "Content with identifier '" + entry.ContentId + "' could not be found.");
            }
        }

        private void ValidateAndPopulateRecordFields(FormEntryDto entry, Core.Models.Form? form, Record record)
        {
            if (form is null) return;

            HttpContext.Items.Add("ApiSubmittedFormId", form!.Id);
            HttpContext.Items.Add("ApiSubmittedFormValues", GetNonFieldValues(form, entry));
            foreach (Core.Models.Field allField in form.AllFields)
            {
                entry.Values.TryGetValue(allField.Alias, out IList<string>? values);
                Core.FieldTypes.FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField is not null)
                {
                    EnsurePrevalues(form, allField);
                    PopulateFieldValue(allField, entry);
                    IEnumerable<object> valuesByFieldTypes = GetFormValuesByFieldTypes(values, allField, fieldTypeByField);
                    foreach (string errorMessage in fieldTypeByField.ValidateField(form, allField, valuesByFieldTypes, HttpContext, _placeholderParsingService, _fieldTypeStorage))
                        ModelState.AddModelError(allField.Alias, errorMessage);
                    RecordField recordField = new(allField);
                    IEnumerable<object> record1 = fieldTypeByField.ConvertToRecord(allField, valuesByFieldTypes, HttpContext);
                    recordField.Values.AddRange(record1);
                    record.RecordFields.Add(allField.Id, recordField);
                }
            }
            FormSubmissionHelper.PublishFormValidateNotification(_eventMessagesFactory, _eventAggregator, HttpContext, ModelState, form);
        }

        private static Dictionary<string, IList<string>> GetNonFieldValues(
          Core.Models.Form form,
          FormEntryDto entryDto)
        {
            Dictionary<string, IList<string>> nonFieldValues = [];
            List<string> list = [.. form.AllFields.Select(x => x.Alias)];
            foreach (KeyValuePair<string, IList<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<string>>>)entryDto.Values)
            {
                if (!list.Contains(keyValuePair.Key))
                    nonFieldValues.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return nonFieldValues;
        }

        /// <summary>Ensures prevalues have been populated on a field.</summary>
        protected void EnsurePrevalues(Core.Models.Form form, Core.Models.Field field) => Task.FromResult(FormRenderingHelper.EnsurePrevalues(form, field, _fieldPreValueSourceService, _fieldPreValueSourceTypeService));

        private static void PopulateFieldValue(Core.Models.Field field, FormEntryDto entry)
        {
            if (!entry.Values.TryGetValue(field.Alias, out IList<string>? source) || source is null)
                return;
            field.Values = [.. source.Cast<object>()];
        }

        private IEnumerable<object> GetFormValuesByFieldTypes(
          IList<string>? values,
          Core.Models.Field field,
          Core.FieldTypes.FieldType fieldType)
        {
            if (values is null || !values.Any())
                return [];
            return (fieldType.Id == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") || fieldType.Id == Guid.Parse("A72C9DF9-3847-47CF-AFB8-B86773FD12CD")) && values[0] == "on" ?
            [
                "true"
            ] : fieldType.SupportsUploadTypes ? ProcessSubmittedFileUpload(values, field) : values;
        }

        private List<object> ProcessSubmittedFileUpload(
          IList<string> values,
          Core.Models.Field field)
        {
            List<object> objectList = [];
            foreach (var input in (IEnumerable<string>)values)
            {
                FormFileUploadDto? formFileUploadDto = _jsonSerializer.Deserialize<FormFileUploadDto>(input);
                if (formFileUploadDto is not null && !string.IsNullOrWhiteSpace(formFileUploadDto.FileName) && !string.IsNullOrWhiteSpace(formFileUploadDto.FileContents))
                {
                    using MemoryStream fromFileContents = CreateStreamFromFileContents(formFileUploadDto.FileContents, out long fileSize);
                    IFormFile formFile = CreateFormFile(formFileUploadDto.FileName, fromFileContents, fileSize);
                    if (!formFile.IsFileTypeAllowed(field, _securitySettings.GetDisallowedFileUploadExtensionsAsArray(), _securitySettings.GetAllowedFileUploadExtensionsAsArray()))
                    {
                        string fileName = formFileUploadDto.FileName;
                        int startIndex = formFileUploadDto.FileName.LastIndexOf('.');
                        string? str = fileName[startIndex..];
                        ModelState.AddModelError(field.Alias, "The file type (" + str + ") you tried to upload is not allowed.");
                    }
                    else
                    {
                        string tempFolder = formFile.SaveUploadedFileToTempFolder(_hostEnvironment);
                        objectList.Add(EncryptFilePath(tempFolder));
                    }
                }
            }
            return objectList;
        }

        private static MemoryStream CreateStreamFromFileContents(
          string fileContents,
          out long fileSize)
        {
            byte[] buffer = Convert.FromBase64String(StripDataUrlPrefix(fileContents));
            fileSize = buffer.Length;
            return new MemoryStream(buffer);
        }

        private static string StripDataUrlPrefix(string fileContents)
        {
            string str = fileContents;
            int startIndex = fileContents.IndexOf("base64,") + "base64,".Length;
            return str[startIndex..];
        }

        private string EncryptFilePath(string path) => path.EncryptFilePath(DataProtector, "***|***");

        private static FormFile CreateFormFile(
          string fileName,
          MemoryStream memoryStream,
          long fileSize)
        {
            return new FormFile(memoryStream, 0L, fileSize, fileName, fileName);
        }

        private void ParsePlaceholdersInValidationMessages(
          ValidationProblemDetails problemDetails,
          Core.Models.Form? form,
          string? contentId,
          IDictionary<string, string?>? additionalData)
        {
            Hashtable? pageElements = GetContentPageElements(contentId);
            foreach (string key in (IEnumerable<string>)problemDetails.Errors.Keys)
            {
                string[] array = [.. problemDetails.Errors[key].Select(x => _placeholderParsingService.ParsePlaceHolders(x, false, form: form, pageElements: pageElements, additionalData: additionalData))];
                problemDetails.Errors[key] = array;
            }
        }
    }
}