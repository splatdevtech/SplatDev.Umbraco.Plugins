
// Type: Umbraco.Forms.Web.Services.FormRenderingService
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

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
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web.Services
{
    internal sealed class FormRenderingService : IFormRenderingService
    {
        private const string FormsFormKey = "umbracoformsform";
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IPageService _pageService;
        private readonly IFormService _formService;
        private readonly IRecordStorage _recordStorage;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly FieldCollection _fieldCollection;
        private readonly AppCaches _appCaches;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly PackageOptionSettings _packageOptionSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly FormDesignSettings _formDesignSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser;
        private readonly HtmlUrlParser _htmlUrlParser;
        private readonly HtmlImageSourceParser _htmlImageSourceParser;
        private IDataProtector? _dataProtector;
        private readonly bool _isFormPrePopulated;

        public FormRenderingService(
          IUmbracoContextAccessor umbracoContextAccessor,
          IPageService pageService,
          IFormService formService,
          IRecordStorage recordStorage,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
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
          HtmlImageSourceParser htmlImageSourceParser)
        {
            this._umbracoContextAccessor = umbracoContextAccessor;
            this._pageService = pageService;
            this._formService = formService;
            this._recordStorage = recordStorage;
            this._fieldTypeStorage = fieldTypeStorage;
            this._fieldPreValueSourceService = fieldPreValueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this._fieldCollection = fieldCollection;
            this._appCaches = appCaches;
            this._hostingEnvironment = hostingEnvironment;
            this._placeholderParsingService = placeholderParsingService;
            this._dataProtectionProvider = dataProtectionProvider;
            this._packageOptionSettings = packageOptionSettings.Value;
            this._securitySettings = securitySettings.Value;
            this._formDesignSettings = formDesignSettings.Value;
            this._eventAggregator = eventAggregator;
            this._eventMessagesFactory = eventMessagesFactory;
            this._tempDataDictionaryFactory = tempDataDictionaryFactory;
            this._htmlLocalLinkParser = htmlLocalLinkParser;
            this._htmlUrlParser = htmlUrlParser;
            this._htmlImageSourceParser = htmlImageSourceParser;
            this._isFormPrePopulated = serviceProvider.GetService(typeof(INotificationHandler<FormPrePopulateNotification>)) != null || serviceProvider.GetService(typeof(INotificationAsyncHandler<FormPrePopulateNotification>)) != null;
        }

        private IDataProtector DataProtector => this._dataProtector ?? (this._dataProtector = this._dataProtectionProvider.CreateProtector("Umbraco.Forms.RecordState"));

        public async Task<FormViewModel> GetFormModelAsync(
          HttpContext httpContext,
          Guid formId,
          Guid? recordId = null,
          string theme = "",
          IDictionary<string, string?>? additionalData = null)
        {
            ITempDataDictionary tempData = this._tempDataDictionaryFactory.GetTempData(httpContext);
            this.PopulatePageElements(httpContext);
            FormViewModel model = FormRenderingService.RetrieveFormModel(tempData);
            if (model == null || model.FormId != formId)
            {
                Form form1 = this.GetForm(formId);
                if (form1 == null)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
                    interpolatedStringHandler.AppendLiteral("Cannot find form with Id ");
                    interpolatedStringHandler.AppendFormatted<Guid>(formId);
                    throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
                Form form = form1;
                model = new FormViewModel();
                if (!string.IsNullOrEmpty(theme))
                    model.Theme = theme;
                this.PrePopulateForm(form, httpContext, model);
                await model.Build(form, this.GetRequestedOrSubmittedRecord(form, recordId, tempData), additionalData, this._fieldTypeStorage, this._fieldPreValueSourceService, this._fieldPreValueSourceTypeService, this._appCaches, this._hostingEnvironment, this._placeholderParsingService, this._packageOptionSettings, this._securitySettings, this._formDesignSettings, this._htmlLocalLinkParser, this._htmlUrlParser, this._htmlImageSourceParser);
                this.ResumeFormState(model, model.FormState);
                if (model.IsFirstPage)
                    this.ClearFormState(model);
                if (this._packageOptionSettings.AllowEditableFormSubmissions)
                {
                    if (recordId.HasValue)
                    {
                        Guid? nullable = recordId;
                        Guid empty = Guid.Empty;
                        if ((nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 1) != 0)
                        {
                            Record record = this.GetRecord(recordId.Value, form);
                            if (record != null)
                            {
                                this.PrePopulateForm(form, httpContext, model, record);
                                model.RecordId = record.UniqueId;
                                model.FormState = record.CreateFormStateFromRecord(form, this._fieldTypeStorage);
                                this.StoreFormState(model.FormState, model);
                                this.ResumeFormState(model, model.FormState, true);
                                goto label_15;
                            }
                            else
                                goto label_15;
                        }
                    }
                    this.ExtractDataFromPages(httpContext, model, form, additionalData);
                }
                else
                    this.ExtractDataFromPages(httpContext, model, form, additionalData);
                label_15:
                form = null;
            }
            else
            {
                if (!string.IsNullOrEmpty(theme))
                    model.Theme = theme;
                this.ResumeFormState(model, model.FormState);
            }
            List<Guid> formIds = this.GetTrackedRenderedFormIds(httpContext, tempData) ?? new List<Guid>();
            if (!formIds.Contains(formId))
                formIds.Add(formId);
            this.SaveTrackedRenderedFormIds(tempData, httpContext, formIds);
            FormViewModel formModelAsync = model;
            tempData = null;
            model = null;
            return formModelAsync;
        }

        private List<Guid>? GetTrackedRenderedFormIds(
          HttpContext httpContext,
          ITempDataDictionary tempData)
        {
            switch (this._packageOptionSettings.TrackRenderedFormsStorageMethod)
            {
                case TrackRenderedFormsStorageMethodOption.TempData:
                    if (tempData["UmbracoForms"] != null)
                        return tempData.Get<List<Guid>>("UmbracoForms");
                    break;
                case TrackRenderedFormsStorageMethodOption.HttpContextItems:
                    object obj;
                    if (httpContext.Items.TryGetValue("UmbracoForms", out obj) && obj is IEnumerable<Guid> source)
                        return source.ToList<Guid>();
                    break;
            }
            return null;
        }

        private void SaveTrackedRenderedFormIds(
          ITempDataDictionary tempData,
          HttpContext httpContext,
          List<Guid> formIds)
        {
            switch (this._packageOptionSettings.TrackRenderedFormsStorageMethod)
            {
                case TrackRenderedFormsStorageMethodOption.TempData:
                    tempData.Put<List<Guid>>("UmbracoForms", formIds);
                    break;
                case TrackRenderedFormsStorageMethodOption.HttpContextItems:
                    httpContext.Items["UmbracoForms"] = formIds;
                    break;
            }
        }

        public void PopulatePageElements(HttpContext httpContext)
        {
            IUmbracoContext umbracoContext;
            if (httpContext.Items.ContainsKey("pageElements") || !this._umbracoContextAccessor.TryGetUmbracoContext(out umbracoContext) || umbracoContext.PublishedRequest?.PublishedContent == null)
                return;
            httpContext.Items["pageElements"] = this._pageService.GetPageElements();
        }

        private static FormViewModel? RetrieveFormModel(ITempDataDictionary tempData) => !tempData.ContainsKey("umbracoformsform") ? null : tempData["umbracoformsform"] as FormViewModel;

        public Form? GetForm(Guid formId) => this._formService.Get(formId);

        public Record? GetRecord(Guid recordId, Form form) => this._recordStorage.GetRecordByUniqueId(recordId, form);

        public void PrePopulateForm(
          Form form,
          HttpContext httpContext,
          FormViewModel model,
          Record? record = null)
        {
            if (!this._isFormPrePopulated)
                return;
            Dictionary<string, object[]> source = this.RetrieveFormState(model);
            Array.Empty<object>();
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
                        if (formField.Values == null)
                            formField.Values = new List<object>();
                        else if (formField.Settings.Keys.Contains("DefaultValue"))
                            formField.Values.Clear();
                        foreach (object obj in objectArray)
                        {
                            if (!obj.Equals(string.Empty))
                                formField.Values.Add(obj);
                            else
                                formField.Values = new List<object>();
                        }
                        goto label_26;
                    }
                }
                objectArray = source.FirstOrDefault<KeyValuePair<string, object[]>>(v => v.Key == formField.Id.ToString()).Value;
                if (objectArray != null)
                {
                    if (formField.Values == null)
                        formField.Values = new List<object>();
                    else if (formField.Settings.Keys.Contains("DefaultValue"))
                        formField.Values.Clear();
                    foreach (object obj in objectArray)
                    {
                        if (!obj.Equals(string.Empty))
                            formField.Values.Add(obj);
                        else
                            formField.Values = new List<object>();
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
            EventMessages messages = this._eventMessagesFactory.Get();
            this._eventAggregator.Publish<FormPrePopulateNotification>(new FormPrePopulateNotification(form, messages));
            if (record == null)
                return;
            foreach (Field allField in form.AllFields)
            {
                object[] postedValues = Array.Empty<object>();
                FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(allField);
                if (fieldTypeByField != null)
                {
                    object[] array = fieldTypeByField.ConvertToRecord(allField, postedValues, httpContext).ToArray<object>();
                    if (record.GetRecordField(allField.Id) == null)
                    {
                        RecordField recordField = new RecordField(allField);
                        recordField.Values.AddRange(array);
                        record.RecordFields.Add(allField.Id, recordField);
                    }
                }
            }
            foreach (KeyValuePair<Guid, RecordField> recordField in record.RecordFields)
            {
                foreach (Field allField in form.AllFields)
                {
                    if (allField.Id == recordField.Value.FieldId && allField.Values != null)
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
            field = form.AllFields.First<Field>(f => f.Id == fieldId);
            fieldType = this._fieldTypeStorage.GetFieldTypeByField(field);
            return fieldType != null;
        }

        private Record? GetRequestedOrSubmittedRecord(
          Form form,
          Guid? requestedRecordId,
          ITempDataDictionary tempData)
        {
            Guid? nullable = null;
            if (requestedRecordId.HasValue && this._packageOptionSettings.AllowEditableFormSubmissions)
            {
                nullable = new Guid?(requestedRecordId.Value);
            }
            else
            {
                object obj;
                if (tempData.TryGetValue("Forms_Current_Record_id", out obj))
                    nullable = (Guid?)obj;
            }
            return nullable.HasValue ? this._recordStorage.GetRecordByUniqueId(nullable.Value, form) : null;
        }

        private void ExtractDataFromPages(
          HttpContext httpContext,
          FormViewModel model,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            model.FormState = this.ExtractPagesState(model, httpContext, form, additionalData);
            this.StoreFormState(model.FormState, model);
            this.ResumeFormState(model, model.FormState, false);
        }

        private Dictionary<string, object[]> ExtractPagesState(
          FormViewModel model,
          HttpContext httpContext,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            Dictionary<string, object[]> dictionary = this.RetrieveFormState(model);
            if (dictionary != null && httpContext.Request.HasFormContentType)
            {
                foreach (PageViewModel page in model.Pages)
                {
                    foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)page.Fieldsets)
                    {
                        foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                        {
                            foreach (FieldViewModel field1 in (IEnumerable<FieldViewModel>)container.Fields)
                            {
                                object[] postedValues = Array.Empty<object>();
                                if (httpContext.Request.Form.Keys.Contains(field1.Name))
                                    postedValues = httpContext.Request.Form[field1.Name].ToObjectArray();
                                Field field2;
                                FieldType fieldType;
                                if (this.TryGetFieldAndFieldType(form, Guid.Parse(field1.Name), out field2, out fieldType))
                                {
                                    field2.PopulateDefaultValue(this._placeholderParsingService, additionalData);
                                    object[] array = fieldType.ProcessSubmittedValue(field2, postedValues, httpContext).ToArray<object>();
                                    if (dictionary.ContainsKey(field1.Name))
                                        dictionary[field1.Name] = array;
                                    else
                                        dictionary.Add(field1.Name, array);
                                }
                            }
                        }
                    }
                }
            }
            return dictionary ?? new Dictionary<string, object[]>();
        }

        public void StoreFormModel(ITempDataDictionary tempData, FormViewModel model) => tempData["umbracoformsform"] = model;

        public void ClearFormModel(ITempDataDictionary tempData) => tempData.Remove("umbracoformsform");

        public Dictionary<string, object[]> GetFormState(
          Form form,
          FormViewModel model,
          HttpContext httpContext)
        {
            return !this._isFormPrePopulated ? this.ExtractCurrentPageState(model, httpContext, form) : this.ExtractAllPagesState(model, httpContext, form);
        }

        private Dictionary<string, object[]> ExtractCurrentPageState(
          FormViewModel model,
          HttpContext httpContext,
          Form form)
        {
            Dictionary<string, object[]> dictionary = this.RetrieveFormState(model);
            if (dictionary != null && model.CurrentPage != null)
            {
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)model.CurrentPage.Fieldsets)
                {
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        foreach (FieldViewModel field1 in (IEnumerable<FieldViewModel>)container.Fields)
                        {
                            object[] postedValues = Array.Empty<object>();
                            if (httpContext.Request.HasFormContentType && httpContext.Request.Form.Keys.Contains(field1.Name))
                                postedValues = httpContext.Request.Form[field1.Name].ToObjectArray();
                            Field field2;
                            FieldType fieldType;
                            if (this.TryGetFieldAndFieldType(form, Guid.Parse(field1.Name), out field2, out fieldType))
                            {
                                object[] array = fieldType.ProcessSubmittedValue(field2, postedValues, httpContext).ToArray<object>();
                                if (dictionary.ContainsKey(field1.Name))
                                    dictionary[field1.Name] = array;
                                else
                                    dictionary.Add(field1.Name, array);
                            }
                        }
                    }
                }
            }
            return dictionary ?? new Dictionary<string, object[]>();
        }

        private Dictionary<string, object[]> ExtractAllPagesState(
          FormViewModel model,
          HttpContext httpContext,
          Form form)
        {
            Dictionary<string, object[]> allPagesState = this.RetrieveFormState(model);
            if (allPagesState == null)
                return new Dictionary<string, object[]>();
            foreach (Field allField in form.AllFields)
            {
                Field field = allField;
                object[] postedValues = Array.Empty<object>();
                object[] objArray1 = form.AllFields.First<Field>(f => f.Id == field.Id).Values == null ? Array.Empty<object>() : form.AllFields.First<Field>(f => f.Id == field.Id).Values.ToArray();
                Field field1;
                FieldType fieldType;
                if (this.TryGetFieldAndFieldType(form, field.Id, out field1, out fieldType))
                {
                    object[] objArray2;
                    Guid id;
                    if (fieldType.SupportsUploadTypes)
                    {
                        objArray2 = objArray1.Length != 0 ? objArray1 : fieldType.ProcessSubmittedValue(field1, postedValues, httpContext).ToArray<object>();
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
                            foreach (string str2 in stringValues)
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

        public Dictionary<string, object[]> RetrieveFormState(FormViewModel model) => string.IsNullOrEmpty(model.RecordState) ? new Dictionary<string, object[]>() : JsonSerializer.Deserialize<Dictionary<string, object[]>>(this.DataProtector.Unprotect(model.RecordState), FormsJsonSerializerOptions.Default) ?? new Dictionary<string, object[]>();

        public void ResumeFormState(
          FormViewModel model,
          Dictionary<string, object[]> state,
          bool editSubmission = false)
        {
            if (state == null)
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
                                if ((id.HasValue ? (id.GetValueOrDefault() == guid ? 1 : 0) : 0) != 0)
                                {
                                    FieldType field2 = this._fieldCollection[Guid.Parse("DA206CAE-1C52-434E-B21A-4A7C198AF877")];
                                    if (field2 != null)
                                    {
                                        field1.FieldType = field2;
                                        field1.HideLabel = true;
                                    }
                                }
                            }
                            if (state.ContainsKey(field1.Name))
                                field1.Values = FormRenderingService.UnpackValues(state[field1.Name]);
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
            if (!(stateValue is JsonElement jsonElement))
                return stateValue;
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    return (object)jsonElement.GetString() ?? string.Empty;
                case JsonValueKind.Number:
                    return jsonElement.GetInt32();
                default:
                    return string.Empty;
            }
        }

        public void StoreFormState(Dictionary<string, object[]> state, FormViewModel model)
        {
            string plaintext = JsonSerializer.Serialize<Dictionary<string, object[]>>(state, FormsJsonSerializerOptions.Default);
            model.RecordState = this.DataProtector.Protect(plaintext);
        }

        public void ClearFormState(FormViewModel model) => model.RecordState = string.Empty;
    }
}
