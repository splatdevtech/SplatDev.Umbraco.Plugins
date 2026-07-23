
// Type: Umbraco.Forms.Web.Api.DeliveryApi.EntriesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Helpers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Models.DeliveryApi;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Attributes;
using Umbraco.Forms.Web.Helpers;
using Umbraco.Forms.Web.ModelBinders;


#nullable enable
namespace Umbraco.Forms.Web.Api.DeliveryApi
{
  [Route("/umbraco/forms/delivery/api/v1/entries")]
  public class EntriesController : FormsDeliveryApiControllerBase
  {
    private readonly IMemberManager _memberManager;
    private readonly IFieldTypeStorage _fieldTypeStorage;
    private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
    private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
    private readonly IPlaceholderParsingService _placeholderParsingService;
    private readonly IRecordService _recordService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly SecuritySettings _securitySettings;
    private readonly PackageOptionSettings _packageOptionSettings;
    private readonly IEventMessagesFactory _eventMessagesFactory;
    private readonly IEventAggregator _eventAggregator;
    private readonly EntryAcceptedDtoFactory _entryAcceptedDtoFactory;
    private IDataProtector? _dataProtector;

    public EntriesController(
      IFormService formService,
      IEntityService entityService,
      IPageService pageService,
      IVariationContextAccessor variationContextAccessor,
      IMemberManager memberManager,
      IFieldTypeStorage fieldTypeStorage,
      IFieldPreValueSourceService fieldPreValueSourceService,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
      IPlaceholderParsingService placeholderParsingService,
      IRecordService recordService,
      IHostEnvironment hostEnvironment,
      IDataProtectionProvider dataProtectionProvider,
      IJsonSerializer jsonSerializer,
      IOptions<SecuritySettings> securitySettings,
      IOptions<PackageOptionSettings> packageOptionSettings,
      IEventMessagesFactory eventMessagesFactory,
      IEventAggregator eventAggregator,
      EntryAcceptedDtoFactory entryAcceptedDtoFactory)
      : base(formService, entityService, pageService, variationContextAccessor)
    {
      this._memberManager = memberManager;
      this._fieldTypeStorage = fieldTypeStorage;
      this._fieldPreValueSourceService = fieldPreValueSourceService;
      this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
      this._placeholderParsingService = placeholderParsingService;
      this._recordService = recordService;
      this._hostEnvironment = hostEnvironment;
      this._dataProtectionProvider = dataProtectionProvider;
      this._jsonSerializer = jsonSerializer;
      this._securitySettings = securitySettings.Value;
      this._packageOptionSettings = packageOptionSettings.Value;
      this._eventMessagesFactory = eventMessagesFactory;
      this._eventAggregator = eventAggregator;
      this._entryAcceptedDtoFactory = entryAcceptedDtoFactory;
    }

    private IDataProtector DataProtector => this._dataProtector ?? (this._dataProtector = this._dataProtectionProvider.CreateProtector("Umbraco.Forms.FileUpload"));

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
    public async Task<ActionResult> SubmitEntry(Guid id, [FromBody, ModelBinder(typeof (UmbracoFormsApiFormEntryDtoModelBinder))] FormEntryDto entry)
    {
      EntriesController entriesController = this;
      if (!string.IsNullOrWhiteSpace(entry.Culture) && !entriesController.TrySetRequestCulture(entry.Culture))
        return (ActionResult) entriesController.BadRequestForInvalidCulture(entry.Culture);
      Form form = entriesController.FormService.Get(id);
      if (form == null)
        return (ActionResult) entriesController.NotFound();
      Record record = new Record();
      FormSubmissionHelper.InitializeRecord(record, form, entriesController.HttpContext, entriesController._packageOptionSettings.EnableRecordingOfIpWithFormSubmission, entry.AdditionalData);
      entriesController.ApplyContentId(entry, record);
      await FormSubmissionHelper.ApplyMemberKey(record, entriesController.HttpContext, entriesController._memberManager);
      entriesController.ValidateAndPopulateRecordFields(entry, form, record);
      if (entriesController.ModelState.IsValid)
      {
        await entriesController._recordService.SubmitAsync(record, form);
        Hashtable contentPageElements = entriesController.GetContentPageElements(entry.ContentId);
        EntryAcceptedDto entryAcceptedDto = entriesController._entryAcceptedDtoFactory.BuildEntryAcceptedDto(form, record, contentPageElements, entry.AdditionalData);
        return (ActionResult) entriesController.Accepted((object) entryAcceptedDto);
      }
      ValidationProblemDetails problemDetails = new ValidationProblemDetails(entriesController.ModelState);
      entriesController.ParsePlaceholdersInValidationMessages(problemDetails, form, entry.ContentId, entry.AdditionalData);
      ObjectResult objectResult = new ObjectResult((object) problemDetails);
      objectResult.ContentTypes.Add("application/problem+json");
      objectResult.StatusCode = new int?(422);
      return (ActionResult) objectResult;
    }

    private void ApplyContentId(FormEntryDto entry, Record record)
    {
      if (string.IsNullOrEmpty(entry.ContentId))
        return;
      int result1;
      if (int.TryParse(entry.ContentId, out result1))
      {
        record.UmbracoPageId = result1;
      }
      else
      {
        Guid result2;
        if (Guid.TryParse(entry.ContentId, out result2))
        {
          Umbraco.Cms.Core.Attempt<int> id = this.EntityService.GetId(result2, UmbracoObjectTypes.Document);
          if (id.Success)
          {
            record.UmbracoPageId = id.Result;
            return;
          }
        }
        this.ModelState.AddModelError("contentKey", "Content with identifier '" + entry.ContentId + "' could not be found.");
      }
    }

    private void ValidateAndPopulateRecordFields(FormEntryDto entry, Form form, Record record)
    {
      this.HttpContext.Items.Add((object) "ApiSubmittedFormId", (object) form.Id);
      this.HttpContext.Items.Add((object) "ApiSubmittedFormValues", (object) EntriesController.GetNonFieldValues(form, entry));
      foreach (Field allField in form.AllFields)
      {
        IList<string> values;
        entry.Values.TryGetValue(allField.Alias, out values);
        FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(allField);
        if (fieldTypeByField != null)
        {
          this.EnsurePrevalues(form, allField);
          EntriesController.PopulateFieldValue(allField, entry);
          IEnumerable<object> valuesByFieldTypes = this.GetFormValuesByFieldTypes(values, allField, fieldTypeByField);
          foreach (string errorMessage in fieldTypeByField.ValidateField(form, allField, valuesByFieldTypes, this.HttpContext, this._placeholderParsingService, this._fieldTypeStorage))
            this.ModelState.AddModelError(allField.Alias, errorMessage);
          RecordField recordField = new RecordField(allField);
          IEnumerable<object> record1 = fieldTypeByField.ConvertToRecord(allField, valuesByFieldTypes, this.HttpContext);
          recordField.Values.AddRange(record1);
          record.RecordFields.Add(allField.Id, recordField);
        }
      }
      FormSubmissionHelper.PublishFormValidateNotification(this._eventMessagesFactory, this._eventAggregator, this.HttpContext, this.ModelState, form);
    }

    private static IDictionary<string, IList<string>> GetNonFieldValues(
      Form form,
      FormEntryDto entryDto)
    {
      Dictionary<string, IList<string>> nonFieldValues = new Dictionary<string, IList<string>>();
      List<string> list = form.AllFields.Select<Field, string>((Func<Field, string>) (x => x.Alias)).ToList<string>();
      foreach (KeyValuePair<string, IList<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<string>>>) entryDto.Values)
      {
        if (!list.Contains(keyValuePair.Key))
          nonFieldValues.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return (IDictionary<string, IList<string>>) nonFieldValues;
    }

    protected void EnsurePrevalues(Form form, Field field) => FormRenderingHelper.EnsurePrevalues(form, field, this._fieldPreValueSourceService, this._fieldPreValueSourceTypeService);

    private static void PopulateFieldValue(Field field, FormEntryDto entry)
    {
      IList<string> source;
      if (!entry.Values.TryGetValue(field.Alias, out source) || source == null)
        return;
      field.Values = source.Cast<object>().ToList<object>();
    }

    private IEnumerable<object> GetFormValuesByFieldTypes(
      IList<string>? values,
      Field field,
      FieldType fieldType)
    {
      if (values == null || !values.Any<string>())
        return Enumerable.Empty<object>();
      return (fieldType.Id == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") || fieldType.Id == Guid.Parse("A72C9DF9-3847-47CF-AFB8-B86773FD12CD")) && values[0] == "on" ? (IEnumerable<object>) new string[1]
      {
        "true"
      } : (fieldType.SupportsUploadTypes ? this.ProcessSubmittedFileUpload(values, field) : (IEnumerable<object>) values);
    }

    private IEnumerable<object> ProcessSubmittedFileUpload(
      IList<string> values,
      Field field)
    {
      List<object> objectList = new List<object>();
      foreach (string input in (IEnumerable<string>) values)
      {
        FormFileUploadDto formFileUploadDto = this._jsonSerializer.Deserialize<FormFileUploadDto>(input);
        if (formFileUploadDto != null && !string.IsNullOrWhiteSpace(formFileUploadDto.FileName) && !string.IsNullOrWhiteSpace(formFileUploadDto.FileContents))
        {
          long fileSize;
          using (MemoryStream fromFileContents = EntriesController.CreateStreamFromFileContents(formFileUploadDto.FileContents, out fileSize))
          {
            IFormFile formFile = EntriesController.CreateFormFile(formFileUploadDto.FileName, fromFileContents, fileSize);
            if (!formFile.IsFileTypeAllowed(field, this._securitySettings.GetDisallowedFileUploadExtensionsAsArray(), this._securitySettings.GetAllowedFileUploadExtensionsAsArray()))
            {
              string fileName = formFileUploadDto.FileName;
              int startIndex = formFileUploadDto.FileName.LastIndexOf('.');
              string str = fileName.Substring(startIndex, fileName.Length - startIndex);
              this.ModelState.AddModelError(field.Alias, "The file type (" + str + ") you tried to upload is not allowed.");
            }
            else
            {
              string tempFolder = formFile.SaveUploadedFileToTempFolder(this._hostEnvironment);
              objectList.Add((object) this.EncryptFilePath(tempFolder));
            }
          }
        }
      }
      return (IEnumerable<object>) objectList;
    }

    private static MemoryStream CreateStreamFromFileContents(
      string fileContents,
      out long fileSize)
    {
      byte[] buffer = Convert.FromBase64String(EntriesController.StripDataUrlPrefix(fileContents));
      fileSize = (long) buffer.Length;
      return new MemoryStream(buffer);
    }

    private static string StripDataUrlPrefix(string fileContents)
    {
      string str = fileContents;
      int startIndex = fileContents.IndexOf("base64,") + "base64,".Length;
      return str.Substring(startIndex, str.Length - startIndex);
    }

    private string EncryptFilePath(string path) => path.EncryptFilePath(this.DataProtector, "***|***");

    private static IFormFile CreateFormFile(
      string fileName,
      MemoryStream memoryStream,
      long fileSize)
    {
      return (IFormFile) new FormFile((Stream) memoryStream, 0L, fileSize, fileName, fileName);
    }

    private void ParsePlaceholdersInValidationMessages(
      ValidationProblemDetails problemDetails,
      Form form,
      string? contentId,
      IDictionary<string, string?>? additionalData)
    {
      Hashtable pageElements = this.GetContentPageElements(contentId);
      foreach (string key in (IEnumerable<string>) problemDetails.Errors.Keys)
      {
        string[] array = ((IEnumerable<string>) problemDetails.Errors[key]).Select<string, string>((Func<string, string>) (x => this._placeholderParsingService.ParsePlaceHolders(x, false, form: form, pageElements: pageElements, additionalData: additionalData))).ToArray<string>();
        problemDetails.Errors[key] = array;
      }
    }
  }
}
