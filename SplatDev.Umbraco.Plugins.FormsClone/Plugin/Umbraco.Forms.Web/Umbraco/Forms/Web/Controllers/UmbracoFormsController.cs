
// Type: Umbraco.Forms.Web.Controllers.UmbracoFormsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Json.Logic;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Templates;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Helpers;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;

#nullable enable
namespace Umbraco.Forms.Web.Controllers
{
    public class UmbracoFormsController : SurfaceController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRecordService _recordService;
        private readonly IFieldTypeStorage _fieldTypeStorage;
        private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
        private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IMemberManager _memberManager;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly IFormRenderingService _formRenderingService;
        private readonly IEventMessagesFactory _eventMessagesFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly PackageOptionSettings _packageOptionSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly FormDesignSettings _formDesignSettings;
        private readonly ILogger<UmbracoFormsController> _logger;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser;
        private readonly HtmlUrlParser _htmlUrlParser;
        private readonly HtmlImageSourceParser _htmlImageSourceParser;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string FormsSubmittedKey = "UmbracoFormSubmitted";
        internal const string FormsSubmittedFromCurrentPageKey = "UmbracoFormSubmittedFromCurrentPage";
        internal const string FormsSubmittedQuerystringKey = "formSubmitted";
        private IDataProtector? _dataProtector;
        private static string controllerPattern = "{({*[\\w]*}*)}";
        public UmbracoFormsController(
          IUmbracoContextAccessor umbracoContextAccessor,
          IUmbracoDatabaseFactory databaseFactory,
          ServiceContext services,
          AppCaches appCaches,
          IProfilingLogger profilingLogger,
          IPublishedUrlProvider publishedUrlProvider,
          IHostingEnvironment hostingEnvironment,
          IRecordService recordService,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          UmbracoHelper umbracoHelper,
          IMemberManager memberManager,
          IPlaceholderParsingService placeholderParsingService,
          IFormRenderingService formRenderingService,
          IEventMessagesFactory eventMessagesFactory,
          IEventAggregator eventAggregator,
          IOptions<PackageOptionSettings> packageOptionSettings,
          IOptions<SecuritySettings> securitySettings,
          IOptions<FormDesignSettings> formDesignSettings,
          ILogger<UmbracoFormsController> logger,
          HtmlLocalLinkParser htmlLocalLinkParser,
          HtmlUrlParser htmlUrlParser,
          HtmlImageSourceParser htmlImageSourceParser,
          IDataProtectionProvider dataProtectionProvider)
          : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._recordService = recordService;
            this._fieldTypeStorage = fieldTypeStorage;
            this._fieldPreValueSourceService = fieldPreValueSourceService;
            this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
            this._umbracoHelper = umbracoHelper;
            this._memberManager = memberManager;
            this._placeholderParsingService = placeholderParsingService;
            this._formRenderingService = formRenderingService;
            this._eventMessagesFactory = eventMessagesFactory;
            this._eventAggregator = eventAggregator;
            this._packageOptionSettings = packageOptionSettings.Value;
            this._securitySettings = securitySettings.Value;
            this._formDesignSettings = formDesignSettings.Value;
            this._logger = logger;
            this._htmlLocalLinkParser = htmlLocalLinkParser;
            this._htmlUrlParser = htmlUrlParser;
            this._htmlImageSourceParser = htmlImageSourceParser;
            this._dataProtectionProvider = dataProtectionProvider;
        }

        private IDataProtector DataProtector => this._dataProtector ?? (this._dataProtector = this._dataProtectionProvider.CreateProtector("Umbraco.Forms.AdditionalData"));

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [ValidateFormsAntiForgeryToken]
        public async Task<IActionResult> HandleForm(FormViewModel model)
        {
            UmbracoFormsController umbracoFormsController1 = this;
            Form form1 = umbracoFormsController1._formRenderingService.GetForm(model.FormId);
            if (form1 == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
                interpolatedStringHandler.AppendLiteral("Could not load form with id ");
                interpolatedStringHandler.AppendFormatted<Guid>(model.FormId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Form form = form1;
            umbracoFormsController1._formRenderingService.PopulatePageElements(umbracoFormsController1.HttpContext);
            IDictionary<string, string> additionalData = null;
            if (!string.IsNullOrEmpty(model.AdditionalData))
                additionalData = JsonSerializer.Deserialize<Dictionary<string, string>>(umbracoFormsController1.DataProtector.Unprotect(model.AdditionalData));
            await model.Build(form, null, additionalData, umbracoFormsController1._fieldTypeStorage, umbracoFormsController1._fieldPreValueSourceService, umbracoFormsController1._fieldPreValueSourceTypeService, umbracoFormsController1.AppCaches, umbracoFormsController1._hostingEnvironment, umbracoFormsController1._placeholderParsingService, umbracoFormsController1._packageOptionSettings, umbracoFormsController1._securitySettings, umbracoFormsController1._formDesignSettings, umbracoFormsController1._htmlLocalLinkParser, umbracoFormsController1._htmlUrlParser, umbracoFormsController1._htmlImageSourceParser);
            if (umbracoFormsController1.HoneyPotIsEmpty(model))
            {
                umbracoFormsController1._formRenderingService.PrePopulateForm(form, umbracoFormsController1.HttpContext, model);
                model.FormState = umbracoFormsController1._formRenderingService.GetFormState(form, model, umbracoFormsController1.HttpContext);
                umbracoFormsController1._formRenderingService.StoreFormState(model.FormState, model);
                umbracoFormsController1._formRenderingService.ResumeFormState(model, model.FormState);
                Dictionary<Guid, string> valuesForConditions = FieldConditionEvaluation.GetFormFieldValuesForConditions(model.FormState, form.AllFields, umbracoFormsController1._fieldTypeStorage, umbracoFormsController1.ControllerContext.HttpContext);
                if (umbracoFormsController1.NavigatingToPreviousPage(model))
                {
                    await umbracoFormsController1.GoBackward(form, model, valuesForConditions, additionalData);
                }
                else
                {
                    if (model.FormStep < model.Pages.Count)
                        umbracoFormsController1.ValidateFormState(model, form, additionalData);
                    if (umbracoFormsController1.ModelState.IsValid)
                        await umbracoFormsController1.GoForward(form, model, valuesForConditions, additionalData);
                }
                model.IsFirstPage = model.FormStep == 0;
                model.IsLastPage = !model.IsMultiPage || !form.ShowSummaryPageOnMultiPageForms ? model.FormStep == form.Pages.Count - 1 : model.FormStep == form.Pages.Count;
            }
            else
            {
                umbracoFormsController1._logger.LogInformation("The form '{FormName}' ({FormId}) was submitted with the honeypot field completed. A bot submission has been assumed. Data will not be stored and workflows will not be run.", form.Name, form.Id);
                model.SubmitHandled = true;
            }
            umbracoFormsController1.OnFormHandled(form, model);
            if (model.SubmitHandled)
            {
                umbracoFormsController1._formRenderingService.ClearFormModel(umbracoFormsController1.TempData);
                umbracoFormsController1._formRenderingService.ClearFormState(model);
                umbracoFormsController1.TempData["UmbracoFormSubmitted"] = model.FormId;
                if (umbracoFormsController1.HttpContext.Items.ContainsKey("FormsRedirectAfterFormSubmitUrl"))
                    return umbracoFormsController1.Redirect(umbracoFormsController1.HttpContext.Items["FormsRedirectAfterFormSubmitUrl"].ToString());
                QueryString querystring1 = QueryString.Create("formSubmitted", model.FormId.ToString());
                Guid? redirectToPageId = model.RedirectToPageId;
                if (redirectToPageId.HasValue)
                {
                    UmbracoFormsController umbracoFormsController2 = umbracoFormsController1;
                    redirectToPageId = model.RedirectToPageId;
                    Guid pageId = redirectToPageId.Value;
                    QueryString querystring2 = querystring1;
                    return umbracoFormsController2.RedirectToProvidedOrConfiguredUmbracoPage(pageId, querystring2);
                }
                IPublishedContent publishedContent = null;
                int result1;
                if (int.TryParse(form.GoToPageOnSubmit, out result1))
                {
                    publishedContent = umbracoFormsController1._umbracoHelper.Content(result1);
                }
                else
                {
                    Guid result2;
                    if (Guid.TryParse(form.GoToPageOnSubmit, out result2))
                        publishedContent = umbracoFormsController1._umbracoHelper.Content(result2);
                }
                if (publishedContent != null)
                    return umbracoFormsController1.RedirectToProvidedOrConfiguredUmbracoPage(publishedContent.Key, querystring1);
                umbracoFormsController1.TempData["UmbracoFormSubmittedFromCurrentPage"] = model.FormId;
                return !umbracoFormsController1._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? umbracoFormsController1.RedirectToCurrentUmbracoPage() : (IActionResult)umbracoFormsController1.RedirectToCurrentUmbracoPage(querystring1);
            }
            umbracoFormsController1._formRenderingService.StoreFormModel(umbracoFormsController1.TempData, model);
            return umbracoFormsController1.CurrentUmbracoPage();
        }

        private bool NavigatingToPreviousPage(FormViewModel model)
        {
            if (model.FormStep == 0 || !this.Request.HasFormContentType)
                return false;
            return !string.IsNullOrEmpty((string)this.Request.Form["__prev"]) || !string.IsNullOrEmpty((string)this.Request.Form["PreviousClicked"]);
        }

        protected virtual void OnFormHandled(Form form, FormViewModel model)
        {
        }

        private async Task GoBackward(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValues,
          IDictionary<string, string?>? additionalData)
        {
            await this.UpdateFormStep(form, model, formFieldValues, additionalData, -1);
        }

        private async Task GoForward(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValues,
          IDictionary<string, string?>? additionalData)
        {
            await this.UpdateFormStep(form, model, formFieldValues, additionalData, 1);
        }

        private async Task UpdateFormStep(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValuesForConditions,
          IDictionary<string, string?>? additionalData,
          int direction)
        {
            model.FormStep += direction;
            model.PageHandled = true;
            if (direction == -1 && model.FormStep <= 0)
                model.FormStep = 0;
            else if (direction == 1 && model.FormStep >= form.Pages.Count)
            {
                if (form.Pages.Count > 0 && this._packageOptionSettings.EnableMultiPageFormSettings && form.ShowSummaryPageOnMultiPageForms && model.FormStep == form.Pages.Count)
                    model.ShowSummaryPage = true;
                else
                    await this.SubmitForm(form, model, formFieldValuesForConditions, additionalData);
            }
            else
            {
                while (model.FormStep < form.Pages.Count && !this.PageHasVisibleFields(form, formFieldValuesForConditions, model.FormStep))
                    await this.UpdateFormStep(form, model, formFieldValuesForConditions, additionalData, direction);
            }
        }

        private bool PageHasVisibleFields(
          Form form,
          IDictionary<Guid, string> formFieldValuesForConditions,
          int formStep)
        {
            List<FieldSet> fieldSets = form.Pages[formStep].FieldSets;
            List<FieldSet> source = new List<FieldSet>();
            foreach (FieldSet formElement in fieldSets)
            {
                if (this.IsFormElementValidForConditions(form, formElement, formFieldValuesForConditions))
                    source.Add(formElement);
            }
            if (!source.Any<FieldSet>())
                return false;
            foreach (Field formElement in source.SelectMany<FieldSet, FieldsetContainer>(x => x.Containers).SelectMany<FieldsetContainer, Field>(x => x.Fields))
            {
                if (this.IsFormElementValidForConditions(form, formElement, formFieldValuesForConditions))
                    return true;
            }
            return false;
        }

        private async Task SubmitForm(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValuesForConditions,
          IDictionary<string, string?>? additionalData)
        {
            UmbracoFormsController umbracoFormsController = this;
            IProfilingLogger profilingLogger = umbracoFormsController.ProfilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 2);
            interpolatedStringHandler.AppendLiteral("Umbraco Forms: Submitting Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<UmbracoFormsController>(stringAndClear))
            {
                model.SubmitHandled = true;
                Record record = new Record();
                if (model.RecordId != Guid.Empty)
                {
                    Record record1 = umbracoFormsController._formRenderingService.GetRecord(model.RecordId, form);
                    if (record1 != null)
                        record = record1;
                }
                FormSubmissionHelper.InitializeRecord(record, form, umbracoFormsController.HttpContext, umbracoFormsController._packageOptionSettings.EnableRecordingOfIpWithFormSubmission, additionalData);
                umbracoFormsController.ApplyUmbracoPageId(record);
                await FormSubmissionHelper.ApplyMemberKey(record, umbracoFormsController.HttpContext, umbracoFormsController._memberManager);
                foreach (FieldSet allFieldSet in form.AllFieldSets)
                {
                    foreach (Field allField in allFieldSet.AllFields)
                    {
                        object[] valueToStore = allField.GetValueToStore(model.FormState, umbracoFormsController._fieldTypeStorage, umbracoFormsController.ControllerContext.HttpContext, true);
                        RecordField recordField1 = record.GetRecordField(allField.Id);
                        if (recordField1 != null)
                        {
                            recordField1.Values.Clear();
                            umbracoFormsController.SetRecordFieldValues(recordField1, form, allFieldSet, allField, formFieldValuesForConditions, valueToStore);
                        }
                        else
                        {
                            RecordField recordField2 = new RecordField(allField);
                            umbracoFormsController.SetRecordFieldValues(recordField2, form, allFieldSet, allField, formFieldValuesForConditions, valueToStore);
                            record.RecordFields.Add(allField.Id, recordField2);
                        }
                    }
                }
                umbracoFormsController._formRenderingService.ClearFormState(model);
                await umbracoFormsController._recordService.SubmitAsync(record, form);
                umbracoFormsController.AddRecordIdToTempData(record);
                record = null;
            }
        }

        private void ApplyUmbracoPageId(Record record)
        {
            Record record1 = record;
            IPublishedContent currentPage = this.CurrentPage;
            int num = currentPage != null ? currentPage.Id : 0;
            record1.UmbracoPageId = num;
        }

        private void SetRecordFieldValues(
          RecordField recordField,
          Form form,
          FieldSet fieldSet,
          Field field,
          IDictionary<Guid, string> formFieldValues,
          object[] fieldValues)
        {
            if (!this.IsFormElementValidForConditions(form, fieldSet, formFieldValues) || !this.IsFormElementValidForConditions(form, field, formFieldValues))
                return;
            recordField.Values.AddRange(fieldValues);
        }

        private bool IsFormElementValidForConditions(
          Form form,
          IConditioned formElement,
          IDictionary<Guid, string> formFieldValues)
        {
            return formElement.Condition == null || formElement.Condition.IsCircular(form) || formElement.Condition.IsVisible(form, this._fieldTypeStorage, formFieldValues, this._placeholderParsingService);
        }

        private void AddRecordIdToTempData(Record record)
        {
            if (this.TempData.ContainsKey("Forms_Current_Record_id"))
                TempData.Remove("Forms_Current_Record_id");
            this.TempData.Add("Forms_Current_Record_id", record.UniqueId);
        }

        private void ValidateFormState(
          FormViewModel model,
          Form form,
          IDictionary<string, string?>? additionalData)
        {
            if (!this.Request.HasFormContentType)
                return;
            UmbracoFormsController.PopulateFieldValues(model, form);
            Dictionary<Guid, string> dictionary = form.AllFields.ToDictionary<Field, Guid, string>(f => f.Id, f => UmbracoFormsController.GetFieldValueForFormStateValidation(f));
            foreach (FieldSet fieldSet in form.Pages[model.FormStep].FieldSets)
            {
                if (fieldSet.Condition == null || !fieldSet.Condition.Enabled || fieldSet.Condition.IsVisible(form, this._fieldTypeStorage, dictionary, this._placeholderParsingService))
                {
                    foreach (Field field in fieldSet.Containers.SelectMany<FieldsetContainer, Field>(c => c.Fields))
                    {
                        FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(field);
                        if (fieldTypeByField != null)
                        {
                            StringValues array = this.Request.Form[field.Id.ToString()];
                            if (fieldTypeByField.SupportsUploadTypes)
                                array = (StringValues)model.FormState[field.Id.ToString()].Select<object, string>(x => x.ToString().Split(new string[1]
                                {
                  "***|***"
                                }, StringSplitOptions.None).Last<string>()).ToArray<string>();
                            foreach (string err in (IEnumerable<string>)((object)fieldTypeByField.ValidateField(form, field, array, this.HttpContext, this._placeholderParsingService, this._fieldTypeStorage) ?? Array.Empty<string>()))
                            {
                                string validationErrorMessage = this._placeholderParsingService.ParsePlaceholdersForValidationErrorMessage(form, field, err);
                                this.ModelState.AddModelError(field.Id.ToString(), validationErrorMessage);
                            }
                        }
                    }
                }
            }
            if (this._packageOptionSettings.EnableAdvancedValidationRules)
            {
                foreach (ValidationRule validationRule in form.ValidationRules)
                {
                    string ruleWithFieldValues = this.GetRuleWithFieldValues(validationRule, model, form);
                    if (UmbracoFormsController.ContainsFieldReferences(ruleWithFieldValues))
                    {
                        this._logger.LogWarning("Could not parse form validation rule as JSON for form {FormName} ({FormId}).", form.Name, form.Id);
                    }
                    else
                    {
                        JsonNode ruleAsJson = JsonNode.Parse(ruleWithFieldValues);
                        if (ruleAsJson == null)
                            this._logger.LogWarning("Could not process form validation rule as JSON for form {FormName} ({FormId}) as it contains unresolved field references.", form.Name, form.Id);
                        else if (!UmbracoFormsController.IsRuleValid(ruleAsJson))
                            this.ModelState.AddModelError(validationRule.FieldId.ToString(), this._placeholderParsingService.ParsePlaceHolders(validationRule.ErrorMessage, false, form: form, additionalData: additionalData));
                    }
                }
            }
            FormSubmissionHelper.PublishFormValidateNotification(this._eventMessagesFactory, this._eventAggregator, this.HttpContext, this.ModelState, form);
        }

        private static bool ContainsFieldReferences(string ruleWithFieldValues)
        {
            string pattern = @"\{(\{[*\w]*\})\}";
            System.Text.RegularExpressions.Regex regex = new(pattern);
            return regex.IsMatch(ruleWithFieldValues);
        }

        private string GetRuleWithFieldValues(
          ValidationRule validationRule,
          FormViewModel model,
          Form form)
        {
            StringBuilder stringBuilder = new StringBuilder(validationRule.Rule);
            foreach (Field allField in form.AllFields)
            {
                string oldValue = "{" + allField.Alias + "}";
                if (validationRule.Rule.Contains(oldValue))
                {
                    string valueFromFormState = this.GetFieldValueFromFormState(model, allField);
                    stringBuilder.Replace(oldValue, valueFromFormState);
                }
            }
            return stringBuilder.ToString();
        }

        private string GetFieldValueFromFormState(FormViewModel model, Field field)
        {
            object[] postedValues = model.FormState[field.Id.ToString()] ?? Array.Empty<object>();
            FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(field);
            return fieldTypeByField == null ? string.Empty : string.Join(",", fieldTypeByField.ConvertToRecord(field, postedValues, this.HttpContext).ToArray<object>().Select<object, string>(new Func<object, string>(this.ToComparibleString)));
        }

        private string ToComparibleString(object value) => value is DateTime dateTime ? dateTime.ToString(CultureInfo.InvariantCulture) : value.ToString() ?? string.Empty;

        private static bool IsRuleValid(JsonNode ruleAsJson)
        {
            JsonNode jsonNode = JsonLogic.Apply(ruleAsJson, (JsonNode)null);
            return jsonNode != null && jsonNode.GetValue<bool>();
        }

        private static string GetFieldValueForFormStateValidation(Field field)
        {
            string lower = true.ToString().ToLower();
            if (field.FieldTypeId == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") && field.Values.Count > 1 && field.Values.First<object>().ToString() == lower)
                field.Values = new List<object>() { lower };
            return string.Join<object>(", ", field.Values ?? new List<object>());
        }

        private static void PopulateFieldValues(FormViewModel model, Form form)
        {
            foreach (Field allField in form.AllFields)
            {
                object[] source;
                model.FormState.TryGetValue(allField.Id.ToString(), out source);
                allField.Values = source != null ? source.ToList<object>() : new List<object>();
            }
        }

        private bool HoneyPotIsEmpty(FormViewModel model) => !this.Request.HasFormContentType || string.IsNullOrEmpty((string)this.Request.Form[model.FormId.ToString().Replace("-", string.Empty)]);

        private IActionResult RedirectToProvidedOrConfiguredUmbracoPage(
          Guid pageId,
          QueryString querystring)
        {
            return this._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? this.RedirectToUmbracoPage(pageId, querystring) : (IActionResult)this.RedirectToUmbracoPage(pageId);
        }
    }
}
