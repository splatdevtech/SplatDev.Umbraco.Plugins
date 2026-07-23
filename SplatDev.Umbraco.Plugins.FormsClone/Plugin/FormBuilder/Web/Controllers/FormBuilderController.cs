using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Evaluators;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Extensions;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Helpers;

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
using System.Text.RegularExpressions;

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
using Umbraco.Cms.Web.Website.ActionResults;
using Umbraco.Cms.Web.Website.Controllers;

namespace FormBuilder.Web.Controllers
{
    /// <summary>
    /// Package surface controller for rendering forms and processing form submissions.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class FormBuilderController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IHostingEnvironment hostEnvironment,
        IRecordService recordService,
        IFieldTypeStorage fieldTypeStorage,
        IFieldPrevalueSourceService fieldPreValueSourceService,
        IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
        UmbracoHelper umbracoHelper,
        IMemberManager memberManager,
        IPlaceholderParsingService placeholderParsingService,
        IFormRenderingService formRenderingService,
        IEventMessagesFactory eventMessagesFactory,
        IEventAggregator eventAggregator,
        IOptions<PackageOptionSettings> packageOptionSettings,
        IOptions<SecuritySettings> securitySettings,
        IOptions<FormDesignSettings> formDesignSettings,
        ILogger<FormBuilderController> logger,
        HtmlLocalLinkParser htmlLocalLinkParser,
        HtmlUrlParser htmlUrlParser,
        HtmlImageSourceParser htmlImageSourceParser,
        IDataProtectionProvider dataProtectionProvider) : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        private readonly IHostingEnvironment _hostEnvironment = hostEnvironment;
        private readonly IRecordService _recordService = recordService;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IFieldPrevalueSourceService _fieldPreValueSourceService = fieldPreValueSourceService;
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
        private readonly UmbracoHelper _umbracoHelper = umbracoHelper;
        private readonly IMemberManager _memberManager = memberManager;
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;
        private readonly IFormRenderingService _formRenderingService = formRenderingService;
        private readonly IEventMessagesFactory _eventMessagesFactory = eventMessagesFactory;
        private readonly IEventAggregator _eventAggregator = eventAggregator;
        private readonly PackageOptionSettings _packageOptionSettings = packageOptionSettings.Value;
        private readonly SecuritySettings _securitySettings = securitySettings.Value;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private readonly ILogger<FormBuilderController> _logger = logger;
        private readonly HtmlLocalLinkParser _htmlLocalLinkParser = htmlLocalLinkParser;
        private readonly HtmlUrlParser _htmlUrlParser = htmlUrlParser;
        private readonly HtmlImageSourceParser _htmlImageSourceParser = htmlImageSourceParser;
        private readonly IDataProtectionProvider _dataProtectionProvider = dataProtectionProvider;
        private const string? FormsSubmittedKey = "FormBuilderSubmitted";
        internal const string? FormsSubmittedFromCurrentPageKey = "FormBuilderSubmittedFromCurrentPage";
        internal const string? FormsSubmittedQuerystringKey = "formSubmitted";
        private IDataProtector? _dataProtector;

        /// <summary>Gets the data protector.</summary>
        /// <value>The data protector.</value>
        /// <remarks>
        /// Must match name used in FormBuilder.ViewComponents.RenderFormViewComponent.
        /// </remarks>
        private IDataProtector DataProtector => _dataProtector ??= _dataProtectionProvider.CreateProtector("FormBuilderAdditionalData");

        /// <summary>Handles a form submission.</summary>
        /// <remarks>
        /// With Umbraco 11/Forms 11 it's possible to disable antiforgery token checks (via Umbraco:Forms:Security:EnableAntiForgeryToken).
        /// When this is set, we don't generate a token when the form is rendered.
        /// When posted we only want to validate if the feature is enabled.  To do that based on the configuration, we need to first
        /// ignore the antiforgery token check such that there's no built-in validation by default.  And then we validate if the feature
        /// is enabled.
        /// </remarks>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [ValidateFormsAntiForgeryToken]
        public async Task<IActionResult> HandleForm(FormViewModel model)
        {
            FormBuilderController FormBuilderController1 = this;
            Form? form1 = FormBuilderController1._formRenderingService.GetForm(model.FormId);
            if (form1 is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(28, 1);
                interpolatedStringHandler.AppendLiteral("Could not load form with id ");
                interpolatedStringHandler.AppendFormatted(model.FormId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Form form = form1;
            FormBuilderController1._formRenderingService.PopulatePageElements(FormBuilderController1.HttpContext);
            IDictionary<string, string>? additionalData = null;
            if (!string.IsNullOrEmpty(model.AdditionalData))
                additionalData = JsonSerializer.Deserialize<Dictionary<string, string>?>(FormBuilderController1.DataProtector.Unprotect(model.AdditionalData));
            await model.Build(form, null, additionalData!, FormBuilderController1._fieldTypeStorage, FormBuilderController1._fieldPreValueSourceService, FormBuilderController1._fieldPreValueSourceTypeService, FormBuilderController1.AppCaches, FormBuilderController1._hostEnvironment, FormBuilderController1._placeholderParsingService, FormBuilderController1._packageOptionSettings, FormBuilderController1._securitySettings, FormBuilderController1._formDesignSettings, FormBuilderController1._htmlLocalLinkParser, FormBuilderController1._htmlUrlParser, FormBuilderController1._htmlImageSourceParser);
            if (FormBuilderController1.HoneyPotIsEmpty(model))
            {
                FormBuilderController1._formRenderingService.PrePopulateForm(form, FormBuilderController1.HttpContext, model);
                model.FormState = FormBuilderController1._formRenderingService.GetFormState(form, model, FormBuilderController1.HttpContext);
                FormBuilderController1._formRenderingService.StoreFormState(model.FormState, model);
                FormBuilderController1._formRenderingService.ResumeFormState(model, model.FormState);
                Dictionary<Guid, string> valuesForConditions = FieldConditionEvaluation.GetFormFieldValuesForConditions(model.FormState, form.AllFields, FormBuilderController1._fieldTypeStorage, FormBuilderController1.ControllerContext.HttpContext);
                if (FormBuilderController1.NavigatingToPreviousPage(model))
                {
                    await FormBuilderController1.GoBackward(form, model, valuesForConditions, additionalData!);
                }
                else
                {
                    if (model.FormStep < model.Pages.Count)
                        FormBuilderController1.ValidateFormState(model, form);
                    if (FormBuilderController1.ModelState.IsValid)
                        await FormBuilderController1.GoForward(form, model, valuesForConditions, additionalData!);
                }
                model.IsFirstPage = model.FormStep == 0;
                model.IsLastPage = !model.IsMultiPage || !form.ShowSummaryPageOnMultiPageForms ? model.FormStep == form.Pages.Count - 1 : model.FormStep == form.Pages.Count;
            }
            else
            {
                FormBuilderController1._logger.LogInformation("The form '{FormName}' ({FormId}) was submitted with the honeypot field completed. A bot submission has been assumed. Data will not be stored and workflows will not be run.", form.Name, form.Id);
                model.SubmitHandled = true;
            }
            FormBuilderController1.OnFormHandled(form, model);
            if (model.SubmitHandled)
            {
                FormBuilderController1._formRenderingService.ClearFormModel(FormBuilderController1.TempData);
                FormBuilderController1._formRenderingService.ClearFormState(model);
                FormBuilderController1.TempData["FormBuilderSubmitted"] = model.FormId;
                if (FormBuilderController1.HttpContext.Items.ContainsKey("FormsRedirectAfterFormSubmitUrl"))
                    return FormBuilderController1.Redirect(FormBuilderController1.HttpContext.Items["FormsRedirectAfterFormSubmitUrl"]!.ToString()!);
                QueryString querystring1 = QueryString.Create("formSubmitted", model.FormId.ToString());
                Guid? redirectToPageId = model.RedirectToPageId;
                if (redirectToPageId.HasValue)
                {
                    FormBuilderController FormBuilderController2 = FormBuilderController1;
                    redirectToPageId = model.RedirectToPageId;
                    Guid pageId = redirectToPageId!.Value;
                    QueryString querystring2 = querystring1;
                    return FormBuilderController2.RedirectToProvidedOrConfiguredUmbracoPage(pageId, querystring2);
                }
                IPublishedContent? publishedContent = null;
                if (int.TryParse(form.GoToPageOnSubmit, out int result1))
                {
                    publishedContent = FormBuilderController1._umbracoHelper.Content(result1);
                }
                else
                {
                    if (Guid.TryParse(form.GoToPageOnSubmit, out Guid result2))
                        publishedContent = FormBuilderController1._umbracoHelper.Content(result2);
                }
                if (publishedContent is not null)
                    return FormBuilderController1.RedirectToProvidedOrConfiguredUmbracoPage(publishedContent.Key, querystring1);
                FormBuilderController1.TempData["FormBuilderSubmittedFromCurrentPage"] = model.FormId;
                return !FormBuilderController1._packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? FormBuilderController1.RedirectToCurrentUmbracoPage() : (IActionResult)FormBuilderController1.RedirectToCurrentUmbracoPage(querystring1);
            }
            FormBuilderController1._formRenderingService.StoreFormModel(FormBuilderController1.TempData, model);
            return FormBuilderController1.CurrentUmbracoPage();
        }

        private bool NavigatingToPreviousPage(FormViewModel model)
        {
            if (model.FormStep == 0 || !Request.HasFormContentType)
                return false;
            return !string.IsNullOrEmpty((string)Request.Form["__prev"]!) || !string.IsNullOrEmpty((string)Request.Form["PreviousClicked"]!);
        }

        /// <summary>Triggered on form handled event.</summary>
        protected virtual void OnFormHandled(Form form, FormViewModel model)
        {
        }

        private async Task GoBackward(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValues,
          IDictionary<string, string?>? additionalData)
        {
            await UpdateFormStep(form, model, formFieldValues, additionalData, -1);
        }

        private async Task GoForward(
          Form form,
          FormViewModel model,
          IDictionary<Guid, string> formFieldValues,
          IDictionary<string, string?>? additionalData)
        {
            await UpdateFormStep(form, model, formFieldValues, additionalData, 1);
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
                if (form.Pages.Count > 0 && _packageOptionSettings.EnableMultiPageFormSettings && form.ShowSummaryPageOnMultiPageForms && model.FormStep == form.Pages.Count)
                    model.ShowSummaryPage = true;
                else
                    await SubmitForm(form, model, formFieldValuesForConditions, additionalData);
            }
            else
            {
                while (model.FormStep < form.Pages.Count && !PageHasVisibleFields(form, formFieldValuesForConditions, model.FormStep))
                    await UpdateFormStep(form, model, formFieldValuesForConditions, additionalData, direction);
            }
        }

        private bool PageHasVisibleFields(
          Form form,
          IDictionary<Guid, string> formFieldValuesForConditions,
          int formStep)
        {
            List<FieldSet> fieldSets = form.Pages[formStep].FieldSets;
            List<FieldSet> source = [];
            foreach (FieldSet formElement in fieldSets)
            {
                if (IsFormElementValidForConditions(form, formElement, formFieldValuesForConditions))
                    source.Add(formElement);
            }
            if (source.Count == 0)
                return false;
            foreach (Field formElement in source.SelectMany(x => x.Containers).SelectMany(x => x.Fields))
            {
                if (IsFormElementValidForConditions(form, formElement, formFieldValuesForConditions))
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
            FormBuilderController FormBuilderController = this;
            IProfilingLogger profilingLogger = FormBuilderController.ProfilingLogger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(44, 2);
            interpolatedStringHandler.AppendLiteral("Umbraco Forms: Submitting Form '");
            interpolatedStringHandler.AppendFormatted(form.Name);
            interpolatedStringHandler.AppendLiteral("' with id '");
            interpolatedStringHandler.AppendFormatted(form.Id);
            interpolatedStringHandler.AppendLiteral("'");
            string? stringAndClear = interpolatedStringHandler.ToStringAndClear();
            using (profilingLogger.DebugDuration<FormBuilderController>(stringAndClear))
            {
                model.SubmitHandled = true;
                Record? record = new();
                if (model.RecordId != Guid.Empty)
                {
                    Record? record1 = FormBuilderController._formRenderingService.GetRecord(model.RecordId, form);
                    if (record1 is not null)
                        record = record1;
                }
                FormSubmissionHelper.InitializeRecord(record, form, FormBuilderController.HttpContext, FormBuilderController._packageOptionSettings.EnableRecordingOfIpWithFormSubmission, additionalData);
                FormBuilderController.ApplyUmbracoPageId(record);
                await FormSubmissionHelper.ApplyMemberKey(record, FormBuilderController.HttpContext, FormBuilderController._memberManager);
                foreach (FieldSet allFieldSet in form.AllFieldSets)
                {
                    foreach (Field allField in allFieldSet.AllFields)
                    {
                        object[] valueToStore = allField.GetValueToStore(model.FormState, FormBuilderController._fieldTypeStorage, FormBuilderController.ControllerContext.HttpContext, true);
                        RecordField? recordField1 = record.GetRecordField(allField.Id);
                        if (recordField1 is not null)
                        {
                            recordField1.Values.Clear();
                            FormBuilderController.SetRecordFieldValues(recordField1, form, allFieldSet, allField, formFieldValuesForConditions, valueToStore);
                        }
                        else
                        {
                            RecordField recordField2 = new(allField);
                            FormBuilderController.SetRecordFieldValues(recordField2, form, allFieldSet, allField, formFieldValuesForConditions, valueToStore);
                            record.RecordFields.Add(allField.Id, recordField2);
                        }
                    }
                }
                FormBuilderController._formRenderingService.ClearFormState(model);
                await FormBuilderController._recordService.SubmitAsync(record, form);
                FormBuilderController.AddRecordIdToTempData(record);
                record = null;
            }
        }

        private void ApplyUmbracoPageId(Record record)
        {
            Record record1 = record;
            IPublishedContent? currentPage = CurrentPage;
            int num = currentPage is not null ? currentPage.Id : 0;
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
            if (!IsFormElementValidForConditions(form, fieldSet, formFieldValues) || !IsFormElementValidForConditions(form, field, formFieldValues))
                return;
            recordField.Values.AddRange(fieldValues);
        }

        private bool IsFormElementValidForConditions(
          Form form,
          Core.Interfaces.IConditioned formElement,
          IDictionary<Guid, string> formFieldValues)
        {
            return formElement.Condition is null || formElement.Condition.IsCircular(form) || formElement.Condition.IsVisible(form, _fieldTypeStorage, formFieldValues, _placeholderParsingService);
        }

        private void AddRecordIdToTempData(Record record)
        {
            TempData.Remove("Forms_Current_Record_id");
            TempData.Add("Forms_Current_Record_id", record.UniqueId);
        }

        private void ValidateFormState(FormViewModel model, Form form)
        {
            if (!Request.HasFormContentType)
                return;
            PopulateFieldValues(model, form);
            Dictionary<Guid, string> dictionary = form.AllFields.ToDictionary(f => f.Id, f => GetFieldValueForFormStateValidation(f));
            foreach (FieldSet fieldSet in form.Pages[model.FormStep].FieldSets)
            {
                if (fieldSet.Condition is null || !fieldSet.Condition.Enabled || fieldSet.Condition.IsVisible(form, _fieldTypeStorage, dictionary, _placeholderParsingService))
                {
                    foreach (Field field in fieldSet.Containers.SelectMany(c => c.Fields))
                    {
                        FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(field);
                        if (fieldTypeByField is not null)
                        {
                            StringValues array = Request.Form[field.Id.ToString()];
                            if (fieldTypeByField.SupportsUploadTypes)
                                array = (StringValues)model.FormState[field.Id.ToString()].Select(x => x.ToString()?.Split(
                                [
                                    "***|***"
                                ], StringSplitOptions.None).Last()).ToArray();
                            foreach (string err in (IEnumerable<string>)((object)fieldTypeByField.ValidateField(form, field, array, HttpContext, _placeholderParsingService, _fieldTypeStorage) ?? Array.Empty<string>()))
                            {
                                string? validationErrorMessage = _placeholderParsingService.ParsePlaceholdersForValidationErrorMessage(form, field, err);
                                ModelState.AddModelError(field.Id.ToString(), validationErrorMessage);
                            }
                        }
                    }
                }
            }
            if (_packageOptionSettings.EnableAdvancedValidationRules)
            {
                foreach (ValidationRule validationRule in form.ValidationRules)
                {
                    string? ruleWithFieldValues = GetRuleWithFieldValues(validationRule, model, form);
                    if (ContainsFieldReferences(ruleWithFieldValues))
                    {
                        _logger.LogWarning("Could not parse form validation rule as JSON for form {FormName} ({FormId}).", form.Name, form.Id);
                    }
                    else
                    {
                        JsonNode? ruleAsJson = JsonNode.Parse(ruleWithFieldValues);
                        if (ruleAsJson is null)
                            _logger.LogWarning("Could not process form validation rule as JSON for form {FormName} ({FormId}) as it contains unresolved field references.", form.Name, form.Id);
                        else if (!IsRuleValid(ruleAsJson))
                            ModelState.AddModelError(validationRule.FieldId.ToString(), validationRule.ErrorMessage);
                    }
                }
            }
            FormSubmissionHelper.PublishFormValidateNotification(_eventMessagesFactory, _eventAggregator, HttpContext, ModelState, form);
        }

        private static bool ContainsFieldReferences(string ruleWithFieldValues)
        {
            string pattern = @"\{(\{[*\w]*\})\}";
            Regex regex = new(pattern);
            return regex.IsMatch(ruleWithFieldValues);
        }

        private string GetRuleWithFieldValues(
          ValidationRule validationRule,
          FormViewModel model,
          Form form)
        {
            StringBuilder stringBuilder = new(validationRule.Rule);
            foreach (Field allField in form.AllFields)
            {
                string? oldValue = "{" + allField.Alias + "}";
                if (validationRule.Rule.Contains(oldValue))
                {
                    string? valueFromFormState = GetFieldValueFromFormState(model, allField);
                    stringBuilder.Replace(oldValue, valueFromFormState);
                }
            }
            return stringBuilder.ToString();
        }

        private string GetFieldValueFromFormState(FormViewModel model, Field field)
        {
            object[] postedValues = model.FormState[field.Id.ToString()] ?? [];
            FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(field);
            return fieldTypeByField is null ? string.Empty : string.Join(",", fieldTypeByField.ConvertToRecord(field, postedValues, HttpContext).ToArray().Select(new Func<object, string>(ToComparibleString)));
        }

        private string ToComparibleString(object value) => value is DateTime dateTime ? dateTime.ToString(CultureInfo.InvariantCulture) : value.ToString() ?? string.Empty;

        private static bool IsRuleValid(JsonNode ruleAsJson)
        {
            JsonNode? jsonNode = JsonLogic.Apply(ruleAsJson, (JsonNode?)null);
            return jsonNode is not null && jsonNode.GetValue<bool>();
        }

        private static string GetFieldValueForFormStateValidation(Field field)
        {
            string? lower = true.ToString().ToLower();
            if (field.FieldTypeId == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") && field.Values.Count > 1 && field.Values.First().ToString() == lower)
                field.Values = [lower];
            return string.Join(", ", field.Values ?? []);
        }

        private static void PopulateFieldValues(FormViewModel model, Form form)
        {
            foreach (Field allField in form.AllFields)
            {
                model.FormState.TryGetValue(allField.Id.ToString(), out object[]? source);
                allField.Values = source is not null ? [.. source] : [];
            }
        }

        private bool HoneyPotIsEmpty(FormViewModel model) => !Request.HasFormContentType || string.IsNullOrEmpty(Request.Form[model.FormId.ToString()?.Replace("-", string.Empty)!]!);

        private RedirectToUmbracoPageResult RedirectToProvidedOrConfiguredUmbracoPage(
          Guid pageId,
          QueryString querystring) => _packageOptionSettings.AppendQueryStringOnRedirectAfterFormSubmission ? RedirectToUmbracoPage(pageId, querystring) : RedirectToUmbracoPage(pageId);
    }
}