using FormBuilder.Core.Configuration;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Core.Storage.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Templates;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace FormBuilder.Core.Services
{
    internal sealed class FormRenderingService(
      IUmbracoContextAccessor umbracoContextAccessor,
      IPageService pageService,
      IFormService formService,
      IRecordStorage recordStorage,
      IFieldTypeStorage fieldTypeStorage,
      IFieldPrevalueSourceService fieldPreValueSourceService,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      FieldCollection fieldCollection,
      AppCaches appCaches,
      IHostingEnvironment hostingEnvironment,
      IPlaceholderParsingService placeholderParsingService,
      ITempDataDictionaryFactory tempDataDictionaryFactory,
      IDataProtectionProvider dataProtectionProvider,
      IOptions<PackageOptionSettings> packageOptionSettings,
      IOptions<SecuritySettings> securitySettings,
      IOptions<FormDesignSettings> formDesignSettings,
      IEventAggregator eventAggregator,
      IEventMessagesFactory eventMessagesFactory,
      IServiceProvider serviceProvider,
      HtmlLocalLinkParser htmlLocalLinkParser,
      HtmlUrlParser htmlUrlParser,
      HtmlImageSourceParser htmlImageSourceParser) : IFormRenderingService
    {
        private const string FormsFormKey = "FormBuildersform";
        private readonly IUmbracoContextAccessor _umbracoContextAccessor = umbracoContextAccessor;
        private readonly IPageService _pageService = pageService;
        private readonly IFormService _formService = formService;
        private readonly IRecordStorage _recordStorage = recordStorage;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IFieldPrevalueSourceService _fieldPreValueSourceService = fieldPreValueSourceService;
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
        private readonly FieldCollection _fieldCollection = fieldCollection;
        private readonly AppCaches _appCaches = appCaches;
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory = tempDataDictionaryFactory;
        private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser = htmlLocalLinkParser;
        private readonly HtmlUrlParser _htmlUrlParser = htmlUrlParser;
        private readonly HtmlImageSourceParser _htmlImageSourceParser = htmlImageSourceParser;
        private IDataProtector? _dataProtector;
        private readonly bool _isFormPrePopulated = serviceProvider.GetService(typeof(INotificationHandler<FormPrePopulateNotification>)) is not null || serviceProvider.GetService(typeof(INotificationAsyncHandler<FormPrePopulateNotification>)) is not null;

        /// <summary>Gets the data protector.</summary>
        /// <value>The data protector.</value>
        /// <remarks>
        /// This is used to protect and unprotect data for the record state.
        /// </remarks>
        private IDataProtector DataProtector => _dataProtector ??= _dataProtectionProvider.CreateProtector("FormBuilderRecordState");

        public async Task<FormViewModel> GetFormModelAsync(
          HttpContext httpContext,
          Guid formId,
          Guid? recordId = null,
          string theme = "",
          IDictionary<string, string?>? additionalData = null)
        {
            ITempDataDictionary? tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            PopulatePageElements(httpContext);
            FormViewModel? model = RetrieveFormModel(tempData);
            if (model is null || model.FormId != formId)
            {
                Form? form1 = GetForm(formId);
                if (form1 is null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(25, 1);
                    interpolatedStringHandler.AppendLiteral("Cannot find form with Id ");
                    interpolatedStringHandler.AppendFormatted(formId);
                    throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
                Form? form = form1;
                model = new FormViewModel();
                if (!string.IsNullOrEmpty(theme))
                    model.Theme = theme;
                PrePopulateForm(form, httpContext, model);
                await model.Build(form, GetRequestedOrSubmittedRecord(form, recordId, tempData), additionalData, _fieldTypeStorage, _fieldPreValueSourceService, _fieldPreValueSourceTypeService, _appCaches, _hostingEnvironment, _placeholderParsingService, _packageOptionSettings, _securitySettings, _formDesignSettings, _htmlLocalLinkParser, _htmlUrlParser, _htmlImageSourceParser);
                ResumeFormState(model, model.FormState);
                if (model.IsFirstPage)
                    ClearFormState(model);
                if (_packageOptionSettings.AllowEditableFormSubmissions)
                {
                    if (recordId.HasValue)
                    {
                        Guid? nullable = recordId;
                        Guid empty = Guid.Empty;
                        if ((nullable.HasValue ? nullable.GetValueOrDefault() != empty ? 1 : 0 : 1) != 0)
                        {
                            Record? record = GetRecord(recordId.Value, form);
                            if (record is not null)
                            {
                                PrePopulateForm(form, httpContext, model, record);
                                model.RecordId = record.UniqueId;
                                model.FormState = record.CreateFormStateFromRecord(form, _fieldTypeStorage);
                                StoreFormState(model.FormState, model);
                                ResumeFormState(model, model.FormState, true);
                                goto label_15;
                            }
                            else
                                goto label_15;
                        }
                    }
                    ExtractDataFromPages(httpContext, model, form, additionalData);
                }
                else
                    ExtractDataFromPages(httpContext, model, form, additionalData);
                label_15:
#pragma warning disable CS0618 // Type or member is obsolete
                form.DisposeIfDisposable();
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                if (!string.IsNullOrEmpty(theme))
                    model.Theme = theme;
                ResumeFormState(model, model.FormState);
            }
            List<Guid> formIds = GetTrackedRenderedFormIds(httpContext, tempData) ?? [];
            if (!formIds.Contains(formId))
                formIds.Add(formId);
            SaveTrackedRenderedFormIds(tempData, httpContext, formIds);
            FormViewModel formModelAsync = model;
            return formModelAsync;
        }

        private List<Guid>? GetTrackedRenderedFormIds(
          HttpContext httpContext,
          ITempDataDictionary tempData)
        {
            switch (_packageOptionSettings.TrackRenderedFormsStorageMethod)
            {
                case TrackRenderedFormsStorageMethodOption.TempData:
                    if (tempData["FormBuilder"] is not null)
                        return tempData.Get<List<Guid>>("FormBuilder");
                    break;

                case TrackRenderedFormsStorageMethodOption.HttpContextItems:
                    if (httpContext.Items.TryGetValue("FormBuilder", out object? obj) && obj is IEnumerable<Guid> source)
                        return [.. source];
                    break;
            }
            return null;
        }

        private void SaveTrackedRenderedFormIds(
          ITempDataDictionary tempData,
          HttpContext httpContext,
          List<Guid> formIds)
        {
            switch (_packageOptionSettings.TrackRenderedFormsStorageMethod)
            {
                case TrackRenderedFormsStorageMethodOption.TempData:
                    tempData.Put("FormBuilder", formIds);
                    break;

                case TrackRenderedFormsStorageMethodOption.HttpContextItems:
                    httpContext.Items["FormBuilder"] = formIds;
                    break;
            }
        }

        /// <inheritdoc />
        public void PopulatePageElements(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey("pageElements") || !_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext) || umbracoContext.PublishedRequest?.PublishedContent is null)
                return;
            httpContext.Items["pageElements"] = _pageService.GetPageElements();
        }

        private static FormViewModel? RetrieveFormModel(ITempDataDictionary tempData) => !tempData.TryGetValue("FormBuildersform", out object? value) ? null : value as FormViewModel;

        public Form? GetForm(Guid formId) => _formService.Get(formId);

        public Record? GetRecord(Guid recordId, Form form) => _recordStorage.GetRecordByUniqueId(recordId, form);

        /// <summary>
        /// Prepopulates a newly created or existing form with custom values.
        /// </summary>
        public void PrePopulateForm(
          Form form,
          HttpContext httpContext,
          FormViewModel model,
          Record? record = null)
        {
            if (!_isFormPrePopulated)
                return;
            Dictionary<string, object[]> source = RetrieveFormState(model);
            foreach (Field allField in form.AllFields)
            {
                Field formField = allField;
                Guid id;
                object[] objectArray;
                if (httpContext.Request.HasFormContentType)
                {
                    ICollection<string> keys = httpContext.Request.Form.Keys;
                    id = formField.Id;
                    string str = id.ToString();
                    if (keys.Contains(str))
                    {
                        IFormCollection form1 = httpContext.Request.Form;
                        id = formField.Id;
                        string key = id.ToString();
                        objectArray = form1[key].ToObjectArray();
                        if (formField.Values is null)
                            formField.Values = [];
                        else if (formField.Settings.ContainsKey("DefaultValue"))
                            formField.Values.Clear();
                        foreach (object obj in objectArray)
                        {
                            if (!obj.Equals(string.Empty))
                                formField.Values.Add(obj);
                            else
                                formField.Values = [];
                        }
                        goto label_26;
                    }
                }
                objectArray = source.FirstOrDefault(v => v.Key == formField.Id.ToString()).Value;
                if (objectArray is not null)
                {
                    if (formField.Values is null)
                        formField.Values = [];
                    else if (formField.Settings.ContainsKey("DefaultValue"))
                        formField.Values.Clear();
                    foreach (object obj in objectArray)
                    {
                        if (!obj.Equals(string.Empty))
                            formField.Values.Add(obj);
                        else
                            formField.Values = [];
                    }
                }
                else
                    continue;
                label_26:
                Dictionary<string, object[]> dictionary1 = source;
                id = formField.Id;
                string key1 = id.ToString();
                if (dictionary1.ContainsKey(key1))
                {
                    Dictionary<string, object[]> dictionary2 = source;
                    id = formField.Id;
                    string key2 = id.ToString();
                    object[] objArray = objectArray;
                    dictionary2[key2] = objArray;
                }
                else
                {
                    Dictionary<string, object[]> dictionary3 = source;
                    id = formField.Id;
                    string key3 = id.ToString();
                    object[] objArray = objectArray;
                    dictionary3.Add(key3, objArray);
                }
            }
            EventMessages messages = _eventMessagesFactory.Get();
            _eventAggregator.Publish(new FormPrePopulateNotification(form, messages));
            if (record is null)
                return;
            foreach (Field allField in form.AllFields)
            {
                object[] postedValues = [];
                FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField is not null)
                {
                    object[] array = [.. fieldTypeByField.ConvertToRecord(allField, postedValues, httpContext)];
                    if (record.GetRecordField(allField.Id) is null)
                    {
                        RecordField recordField = new(allField);
                        recordField.Values.AddRange(array);
                        record.RecordFields.Add(allField.Id, recordField);
                    }
                }
            }
            foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
            {
                foreach (Field allField in form.AllFields)
                {
                    if (allField.Id == recordField.Value.FieldId && allField.Values is not null)
                        recordField.Value.Values = allField.Values;
                }
            }
        }

        private bool TryGetFieldAndFieldType(
          Form form,
          Guid fieldId,
          out Field field,
          [NotNullWhen(true)] out FieldType? fieldType)
        {
            field = form.AllFields.First(f => f.Id == fieldId);
            fieldType = _fieldTypeStorage.GetFieldTypeByField(field);
            return fieldType is not null;
        }

        /// <summary>Finds the requested or just submitted record.</summary>
        private Record? GetRequestedOrSubmittedRecord(
          Form form,
          Guid? requestedRecordId,
          ITempDataDictionary tempData)
        {
            Guid? nullable = new Guid?();
            if (requestedRecordId.HasValue && _packageOptionSettings.AllowEditableFormSubmissions)
            {
                nullable = new Guid?(requestedRecordId.Value);
            }
            else
            {
                if (tempData.TryGetValue("Forms_Current_Record_id", out object? obj))
                    nullable = (Guid?)obj;
            }
            return nullable.HasValue ? _recordStorage.GetRecordByUniqueId(nullable.Value, form) : null;
        }

        /// <summary>
        /// This method extracts the data from all the pages then saves and resumes the state
        /// </summary>
        private void ExtractDataFromPages(
          HttpContext httpContext,
          FormViewModel model,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            model.FormState = ExtractPagesState(model, httpContext, form, additionalData);
            StoreFormState(model.FormState, model);
            ResumeFormState(model, model.FormState, false);
        }

        /// <summary>Extracts the data from all the pages</summary>
        private Dictionary<string, object[]> ExtractPagesState(
          FormViewModel model,
          HttpContext httpContext,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            Dictionary<string, object[]> dictionary = RetrieveFormState(model);
            if (dictionary is not null && httpContext.Request.HasFormContentType)
            {
                foreach (PageViewModel page in model.Pages)
                {
                    foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)page.Fieldsets)
                    {
                        foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                        {
                            foreach (FieldViewModel field1 in (IEnumerable<FieldViewModel>)container.Fields)
                            {
                                object[] postedValues = [];
                                if (httpContext.Request.Form.Keys.Contains(field1.Name))
                                    postedValues = httpContext.Request.Form[field1.Name].ToObjectArray();
                                if (TryGetFieldAndFieldType(form, Guid.Parse(field1.Name), out Field field2, out FieldType? fieldType))
                                {
                                    field2.PopulateDefaultValue(_placeholderParsingService, additionalData);
                                    object[] array = [.. fieldType.ProcessSubmittedValue(field2, postedValues, httpContext)];
                                    if (!dictionary.TryAdd(field1.Name, array))
                                        dictionary[field1.Name] = array;
                                }
                            }
                        }
                    }
                }
            }
            return dictionary ?? [];
        }

        public void StoreFormModel(ITempDataDictionary tempData, FormViewModel model) => tempData["FormBuildersform"] = model;

        public void ClearFormModel(ITempDataDictionary tempData) => tempData.Remove("FormBuildersform");

        public Dictionary<string, object[]> GetFormState(
          Form form,
          FormViewModel model,
          HttpContext httpContext)
        {
            return !_isFormPrePopulated ? ExtractCurrentPageState(model, httpContext, form) : ExtractAllPagesState(model, httpContext, form);
        }

        private Dictionary<string, object[]> ExtractCurrentPageState(
          FormViewModel model,
          HttpContext httpContext,
          Form form)
        {
            Dictionary<string, object[]> dictionary = RetrieveFormState(model);
            if (dictionary is not null && model.CurrentPage is not null)
            {
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)model.CurrentPage.Fieldsets)
                {
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        foreach (FieldViewModel field1 in (IEnumerable<FieldViewModel>)container.Fields)
                        {
                            object[] postedValues = [];
                            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.Keys.Contains(field1.Name))
                                postedValues = httpContext.Request.Form[field1.Name].ToObjectArray();
                            if (TryGetFieldAndFieldType(form, Guid.Parse(field1.Name), out Field field2, out FieldType? fieldType))
                            {
                                object[] array = [.. fieldType.ProcessSubmittedValue(field2, postedValues, httpContext)];
                                if (!dictionary.TryAdd(field1.Name, array))
                                    dictionary[field1.Name] = array;
                            }
                        }
                    }
                }
            }
            return dictionary ?? [];
        }

        /// <summary>
        /// Needed when pre-population is used as we need the state of all fields and not only the ones on the  current page
        /// </summary>
        private Dictionary<string, object[]> ExtractAllPagesState(
          FormViewModel model,
          HttpContext httpContext,
          Form form)
        {
            Dictionary<string, object[]> allPagesState = RetrieveFormState(model);
            if (allPagesState is null)
                return [];
            foreach (Field allField in form.AllFields)
            {
                Field field = allField;
                object[] postedValues = [];
                object[] objArray1 = form.AllFields.First(f => f.Id == field.Id).Values is null ? [] : [.. form.AllFields.First<Field>(f => f.Id == field.Id).Values];
                if (TryGetFieldAndFieldType(form, field.Id, out Field field1, out FieldType? fieldType))
                {
                    object[] objArray2;
                    Guid id;
                    if (fieldType.SupportsUploadTypes)
                    {
                        objArray2 = objArray1.Length != 0 ? objArray1 : [.. fieldType.ProcessSubmittedValue(field1, postedValues, httpContext)];
                    }
                    else
                    {
                        ICollection<string> keys = httpContext.Request.Form.Keys;
                        id = field.Id;
                        string str1 = id.ToString();
                        if (keys.Contains(str1))
                        {
                            IFormCollection form1 = httpContext.Request.Form;
                            id = field.Id;
                            string key = id.ToString();
                            StringValues stringValues = form1[key];
                            bool flag = true;
                            foreach (string? str2 in stringValues)
                            {
                                if (!string.IsNullOrEmpty(str2))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            objArray2 = flag ? objArray1 : stringValues.ToObjectArray();
                        }
                        else
                            objArray2 = objArray1;
                    }
                    Dictionary<string, object[]> dictionary1 = allPagesState;
                    id = field.Id;
                    string key1 = id.ToString();
                    if (dictionary1.ContainsKey(key1))
                    {
                        Dictionary<string, object[]> dictionary2 = allPagesState;
                        id = field.Id;
                        string key2 = id.ToString();
                        object[] objArray3 = objArray2;
                        dictionary2[key2] = objArray3;
                    }
                    else
                    {
                        Dictionary<string, object[]> dictionary3 = allPagesState;
                        id = field.Id;
                        string key3 = id.ToString();
                        object[] objArray4 = objArray2;
                        dictionary3.Add(key3, objArray4);
                    }
                }
            }
            return allPagesState;
        }

        public Dictionary<string, object[]> RetrieveFormState(FormViewModel model) => string.IsNullOrEmpty(model.RecordState) ? [] : JsonSerializer.Deserialize<Dictionary<string, object[]>>(DataProtector.Unprotect(model.RecordState), FormsJsonSerializerOptions.Default) ?? [];

        /// <summary>
        /// For repopulating the form object with data entered previously
        /// </summary>
        public void ResumeFormState(
          FormViewModel model,
          Dictionary<string, object[]> state,
          bool editSubmission = false)
        {
            if (state is null)
                return;
            foreach (PageViewModel page in model.Pages)
            {
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)page.Fieldsets)
                {
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        foreach (FieldViewModel field1 in (IEnumerable<FieldViewModel>)container.Fields)
                        {
                            if (editSubmission)
                            {
                                Guid? id = field1.FieldType?.Id;
                                Guid guid = Guid.Parse("A72C9DF9-3847-47CF-AFB8-B86773FD12CD");
                                if ((id.HasValue ? id.GetValueOrDefault() == guid ? 1 : 0 : 0) != 0)
                                {
                                    FieldType? field2 = _fieldCollection[Guid.Parse("DA206CAE-1C52-434E-B21A-4A7C198AF877")];
                                    if (field2 is not null)
                                    {
                                        field1.FieldType = field2;
                                        field1.HideLabel = true;
                                    }
                                }
                            }
                            if (state.TryGetValue(field1.Name, out object[]? value))
                                field1.Values = UnpackValues(value);
                        }
                    }
                }
            }
        }

        private static IEnumerable<object> UnpackValues(object[] stateValues)
        {
            return stateValues.Select(UnpackValue);
        }

        private static object UnpackValue(object stateValue)
        {
            if (stateValue is JsonElement jsonElement)
            {
                return jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                    JsonValueKind.Number => jsonElement.GetInt32(),
                    _ => string.Empty
                };
            }
            return stateValue;
        }

        public void StoreFormState(Dictionary<string, object[]> state, FormViewModel model)
        {
            string plaintext = JsonSerializer.Serialize(state, FormsJsonSerializerOptions.Default);
            model.RecordState = DataProtector.Protect(plaintext);
        }

        public void ClearFormState(FormViewModel model) => model.RecordState = string.Empty;
    }
}