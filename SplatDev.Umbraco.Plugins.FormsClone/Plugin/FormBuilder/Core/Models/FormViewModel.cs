using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Extension.Forms.Core.Helpers;

using Microsoft.AspNetCore.Html;

using System.Globalization;
using System.Text;
using System.Web;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Templates;
using Umbraco.Extensions;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a form.</summary>
    [Serializable]
    public class FormViewModel
    {
        /// <summary>Gets or sets the form's Id.</summary>
        public Guid FormId { get; set; }

        /// <summary>Gets or sets the form's client Id.</summary>
        public string FormClientId { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's name.</summary>
        public string FormName { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's pages.</summary>
        public List<PageViewModel> Pages { get; set; } = [];

        /// <summary>Gets or sets the form's validation rules.</summary>
        public List<ValidationRuleViewModel> ValidationRules { get; set; } = [];

        /// <summary>Gets or sets the form's step number.</summary>
        public int FormStep { get; set; }

        /// <summary>
        /// Gets or sets the form's associated record (form submission) Id.
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>Gets or sets the form's them.</summary>
        /// <remarks>
        /// WIP: adding a theme alias which will default to /partials/forms/theme/alias/form.cshtml
        /// </remarks>
        public string? Theme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render the script files.
        /// </summary>
        public bool RenderScriptFiles { get; set; }

        /// <summary>Gets or sets the form's state.</summary>
        public Dictionary<string, object[]> FormState { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether the form has been paged (i.e. the user has moved on or back following the initial presentation of the form).
        /// </summary>
        public bool PageHandled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form has been submitted.
        /// </summary>
        public bool SubmitHandled { get; set; }

        /// <summary>Gets or sets the form's field indicator.</summary>
        public string Indicator { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether field validation should be shown.
        /// </summary>
        public bool ShowFieldValidaton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation summary should be shown.
        /// </summary>
        public bool ShowValidationSummary { get; set; }

        /// <summary>
        /// Gets or sets the form's message to display following submission,
        /// </summary>
        public string MessageOnSubmit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the form's message to display on submission is in HTML format.
        /// </summary>
        public bool MessageOnSubmitIsHtml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether default stylesheet should be disabled.
        /// </summary>
        public bool DisableDefaultStylesheet { get; set; }

        /// <summary>Gets or sets the form's previous clicked value.</summary>
        public string? PreviousClicked { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded serialized (and encrypted) record state.
        /// </summary>
        public string RecordState { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to render the antiforgery token.
        /// </summary>
        public bool RenderAntiForgeryToken { get; set; }

        /// <summary>
        /// Gets or sets the serialized (and encrypted) additional data (that will be made available to workflows).
        /// </summary>
        public string? AdditionalData { get; set; }

        /// <summary>Gets the form's current page..</summary>
        public PageViewModel? CurrentPage => Pages.Count >= FormStep + 1 ? Pages[FormStep] : null;

        /// <summary>
        /// Gets or sets a value indicating whether the form is a multi-page form.
        /// </summary>
        public bool IsMultiPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form's first page is being displayed.
        /// </summary>
        public bool IsFirstPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form's last page is being displayed.
        /// </summary>
        public bool IsLastPage { get; set; }

        /// <summary>Gets the page number.</summary>
        public int PageNumber => FormStep + 1;

        /// <summary>Gets the number of pages.</summary>
        public int PageCount => Pages.Count + (HasSummaryPage ? 1 : 0);

        /// <summary>Gets or sets the form's CSS class.</summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's submit button caption.</summary>
        public string SubmitCaption { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's next page button caption.</summary>
        public string NextCaption { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's previous page button caption.</summary>
        public string PreviousCaption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether paging details on multi-page forms should be displayed at the top of the form.
        /// </summary>
        public bool ShowPagingOnMultiPageFormsAtTop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether paging details on multi-page forms should be displayed at the bottom of the form.
        /// </summary>
        public bool ShowPagingOnMultiPageFormsAtBottom { get; set; }

        /// <summary>Gets or sets the paging details format.</summary>
        public string PagingDetailsFormat { get; set; } = "Page {0} of {1}";

        /// <summary>Gets or sets the default page caption format.</summary>
        /// <remarks>
        /// This will be used when the page isn't provided with a caption.
        /// </remarks>
        public string PageCaptionFormat { get; set; } = "Page {0}";

        /// <summary>
        /// Gets or sets a value indicating whether the multi-page form summary will be displayed.
        /// </summary>
        public bool HasSummaryPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the multi-page form summary should be displayed.
        /// </summary>
        public bool ShowSummaryPage { get; set; }

        /// <summary>Gets or sets the form's next page button caption.</summary>
        public string SummaryCaption { get; set; } = string.Empty;

        /// <summary>Gets or sets the form's page button conditions.</summary>
        public Dictionary<Guid, ConditionViewModel> PageButtonConditions { get; set; } = [];

        /// <summary>Gets or sets the form's fieldset conditions.</summary>
        public Dictionary<Guid, ConditionViewModel> FieldsetConditions { get; set; } = [];

        /// <summary>Gets or sets the form's field conditions.</summary>
        public Dictionary<Guid, ConditionViewModel> FieldConditions { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating the client-side event on which condition checks should be triggered.
        /// </summary>
        /// <remarks>Valid values are "change" and "input".</remarks>
        public string TriggerConditionsCheckOn { get; set; } = "change";

        /// <summary>
        /// Gets or sets a value indicating the HTML ID prefix used when rendering form fields.
        /// </summary>
        public string FormElementHtmlIdPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the HTML attributes to apply to the rendered form.
        /// </summary>
        public IDictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the HTML attributes that will be applied to the {script} tags rendered.
        /// </summary>
        /// <remarks>
        /// By default forms scripts will have the defer attribute applied. This should have no adverse side affects since
        /// the forms scripts don't rely on in-line script processing, they will all happily execute when the document is ready.
        /// </remarks>
        public IDictionary<string, string> JavaScriptTagAttributes { get; set; } = new Dictionary<string, string>()
    {
      {
        "defer",
        "defer"
      }
    };

        /// <summary>
        /// Gets or sets a value indicating whether to disable the client side validation framework dependency check.
        /// </summary>
        public bool DisableClientSideValidationDependencyCheck { get; set; }

        /// <summary>
        /// Gets or sets the Node ID of the custom redirect page after submitting a form.
        /// </summary>
        public Guid? RedirectToPageId { get; set; }

        /// <summary>
        /// Retrieves a list of fields not currently displayed on the current form step.
        /// </summary>
        public IDictionary<string, string> GetFieldsNotDisplayed(
          string formElementHtmlIdPrefix)
        {
            return Pages.Select((x, index) => new
            {
                item = x,
                index
            }).Where(x => x.index != FormStep).Select(x => x.item).SelectMany(x => x.Fieldsets).SelectMany(x => x.Containers).SelectMany(x => x.Fields).ToDictionary(f => formElementHtmlIdPrefix + f.Name, f => f.Values is null ? string.Empty : string.Join(";;", f.Values));
        }

        /// <summary>
        /// Retrieves and appropriately encodes the message show on form on submission.
        /// </summary>
        public IHtmlContent GetMessageOnSubmit() => new HtmlString(MessageOnSubmitIsHtml ? MessageOnSubmit : HttpUtility.HtmlEncode(MessageOnSubmit));

        /// <summary>
        /// Retrieves the caption for the page at the provided index.
        /// </summary>
        /// <param name="index">Page index.</param>
        /// <returns>Caption of page.</returns>
        public string GetPageCaption(int index)
        {
            if (index >= Pages.Count)
                return string.Empty;
            return !string.IsNullOrWhiteSpace(Pages[index].Caption) ? Pages[index].Caption : string.Format(PageCaptionFormat, index + 1);
        }

        /// <summary>
        /// Builds the view model from the provided form and services.
        /// </summary>
        public async Task Build(
          Form form,
          Record? record,
          IDictionary<string, string?>? additionalData,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPrevalueSourceService fieldPreValueSourceService,
          IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
          AppCaches appCaches,
          IHostingEnvironment hostingEnvironment,
          IPlaceholderParsingService placeholderParsingService,
          PackageOptionSettings packageOptionSettings,
          SecuritySettings securitySettings,
          FormDesignSettings formDesignSettings,
          HtmlLocalLinkParser htmlLocalLinkParser,
          HtmlUrlParser htmlUrlParser,
          HtmlImageSourceParser htmlImageSourceParser)
        {
            FormViewModel formViewModel = this;
            formViewModel.FormId = form.Id;
            formViewModel.FormClientId = form.Id.ToString().Replace("-", string.Empty);
            formViewModel.FormName = form.Name;
            formViewModel.Indicator = placeholderParsingService.ParsePlaceHolders(form.Indicator, false, record, form, additionalData: additionalData);
            formViewModel.ShowFieldValidaton = !form.HideFieldValidation;
            formViewModel.ShowValidationSummary = form.ShowValidationSummary;
            string text = placeholderParsingService.ParsePlaceHolders(form.MessageOnSubmit ?? string.Empty, form.MessageOnSubmitIsHtml, record, form, additionalData: additionalData);
            formViewModel.MessageOnSubmitIsHtml = form.MessageOnSubmitIsHtml;
            if (form.MessageOnSubmitIsHtml)
                text = htmlImageSourceParser.EnsureImageSources(htmlUrlParser.EnsureUrls(htmlLocalLinkParser.EnsureInternalLinks(text)));
            formViewModel.MessageOnSubmit = text;
            formViewModel.IsMultiPage = form.Pages.Count > 1;
            formViewModel.IsFirstPage = formViewModel.FormStep == 0;
            formViewModel.IsLastPage = formViewModel.FormStep == form.Pages.Count - 1;
            formViewModel.PreviousClicked = string.Empty;
            formViewModel.DisableDefaultStylesheet = form.DisableDefaultStylesheet;
            formViewModel.CssClass = (XmlHelper.XmlName(form.Name) + " " + form.CssClass).Trim();
            if (!string.IsNullOrWhiteSpace(form.AutocompleteAttribute) && !formViewModel.HtmlAttributes.ContainsKey("autocomplete"))
                formViewModel.HtmlAttributes.Add("autocomplete", form.AutocompleteAttribute);
            string str1 = string.IsNullOrWhiteSpace(form.SubmitLabel) ? "Submit" : placeholderParsingService.ParsePlaceHolders(form.SubmitLabel, false, record, form, additionalData: additionalData);
            formViewModel.SubmitCaption = str1;
            string str2 = string.IsNullOrWhiteSpace(form.PrevLabel) ? "Previous" : placeholderParsingService.ParsePlaceHolders(form.PrevLabel, false, record, form, additionalData: additionalData);
            formViewModel.PreviousCaption = str2;
            string str3 = string.IsNullOrWhiteSpace(form.NextLabel) ? "Next" : placeholderParsingService.ParsePlaceHolders(form.NextLabel, false, record, form, additionalData: additionalData);
            formViewModel.NextCaption = str3;
            string conditionsCheckOn = packageOptionSettings.TriggerConditionsCheckOn;
            formViewModel.TriggerConditionsCheckOn = string.IsNullOrWhiteSpace(conditionsCheckOn) ? "change" : conditionsCheckOn.ToLowerInvariant();
            formViewModel.RenderAntiForgeryToken = securitySettings.EnableAntiForgeryToken;
            formViewModel.FormElementHtmlIdPrefix = formDesignSettings.FormElementHtmlIdPrefix;
            formViewModel.DisableClientSideValidationDependencyCheck = packageOptionSettings.DisableClientSideValidationDependencyCheck;
            if (packageOptionSettings.EnableMultiPageFormSettings)
            {
                formViewModel.ShowPagingOnMultiPageFormsAtTop = form.ShowPagingOnMultiPageForms.HasFlag(MultiPageNavigationOption.ShowAtTop);
                formViewModel.ShowPagingOnMultiPageFormsAtBottom = form.ShowPagingOnMultiPageForms.HasFlag(MultiPageNavigationOption.ShowAtBottom);
                string str4 = string.IsNullOrWhiteSpace(form.PagingDetailsFormat) ? "Page {0} of {1}" : placeholderParsingService.ParsePlaceHolders(form.PagingDetailsFormat, false, record, form, additionalData: additionalData);
                formViewModel.PagingDetailsFormat = str4;
                string str5 = string.IsNullOrWhiteSpace(form.PageCaptionFormat) ? "Page {0}" : placeholderParsingService.ParsePlaceHolders(form.PageCaptionFormat, false, record, form, additionalData: additionalData);
                formViewModel.PageCaptionFormat = str5;
                formViewModel.HasSummaryPage = form.ShowSummaryPageOnMultiPageForms;
                string str6 = string.IsNullOrWhiteSpace(form.SummaryLabel) ? "Summary of Entry" : placeholderParsingService.ParsePlaceHolders(form.SummaryLabel, false, record, form, additionalData: additionalData);
                formViewModel.SummaryCaption = str6;
            }
            int fldCount = 1;
            foreach (Page page in form.Pages)
            {
                PageViewModel? pageModel = new()
                {
                    Id = page.Id,
                    Caption = placeholderParsingService.ParsePlaceHolders(page.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                    HasButtonCondition = page.ButtonCondition is not null && page.ButtonCondition.Enabled,
                    ButtonCondition = page.ButtonCondition,
                    ButtonConditionActionType = page.ButtonCondition is not null ? page.ButtonCondition.ActionType : FieldConditionActionType.Show,
                    ButtonConditionLogicType = page.ButtonCondition is not null ? page.ButtonCondition.LogicType : FieldConditionLogicType.Any,
                    ButtonConditionRules = page.ButtonCondition is not null ? [.. page.ButtonCondition.Rules] : []
                };
                if (page.ButtonCondition is not null && page.ButtonCondition.Enabled)
                    formViewModel.PageButtonConditions.Add(page.Id, ConditionViewModel.Build(form, page.ButtonCondition, placeholderParsingService));
                if (!form.DisableDefaultStylesheet)
                    pageModel.CssFiles.Add("default", "~/App_Plugins/FormBuilder/assets/defaultform.min.css");
                foreach (FieldSet fieldset in page.FieldSets)
                {
                    FieldsetViewModel? fieldsetModel = new()
                    {
                        Id = formDesignSettings.FormElementHtmlIdPrefix + fieldset.Id.ToString(),
                        Caption = placeholderParsingService.ParsePlaceHolders(fieldset.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                        HasCondition = fieldset.Condition is not null && fieldset.Condition.Enabled,
                        Condition = fieldset.Condition,
                        ConditionActionType = fieldset.Condition is not null ? fieldset.Condition.ActionType : FieldConditionActionType.Show,
                        ConditionLogicType = fieldset.Condition is not null ? fieldset.Condition.LogicType : FieldConditionLogicType.Any,
                        ConditionRules = fieldset.Condition is not null ? [.. fieldset.Condition.Rules] : []
                    };
                    if (fieldset.Condition is not null && fieldset.Condition.Enabled)
                        formViewModel.FieldsetConditions.Add(fieldset.Id, ConditionViewModel.Build(form, fieldset.Condition, placeholderParsingService));
                    foreach (FieldsetContainer container in fieldset.Containers)
                    {
                        FieldsetContainerViewModel? fieldsetContainerModel = new()
                        {
                            Caption = placeholderParsingService.ParsePlaceHolders(container.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                            Width = container.Width
                        };
                        foreach (Field field1 in container.Fields)
                        {
                            Field? field = field1;
                            FieldType? fieldType = fieldTypeStorage.GetFieldTypeByField(field);
                            if (fieldType is not null)
                            {
                                if (field.Condition is not null && field.Condition.Enabled)
                                    formViewModel.FieldConditions.Add(field.Id, ConditionViewModel.Build(form, field.Condition, placeholderParsingService));
                                field.PopulateDefaultValue(placeholderParsingService, additionalData);
                                string placeHolders = placeholderParsingService.ParsePlaceHolders(field.Caption, false, record, form, additionalData: additionalData);
                                FieldViewModel? fieldViewModel1 = new()
                                {
                                    Alias = field.Alias,
                                    Id = formDesignSettings.FormElementHtmlIdPrefix + field.Id.ToString(),
                                    FieldsetId = fieldset.Id,
                                    PageId = page.Id,
                                    FormId = form.Id,
                                    Name = field.Id.ToString(),
                                    Caption = placeHolders,
                                    RequiredErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, placeholderParsingService, form, additionalData),
                                    InvalidErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, placeholderParsingService, form, additionalData),
                                    FieldTypeName = fieldType?.Name ?? string.Empty,
                                    FieldType = fieldType
                                };
                                FieldViewModel? fieldViewModel = fieldViewModel1;
                                FieldType? fieldType1 = fieldType;
                                int num = fieldType1 is not null ? fieldType1.HideLabel ? 1 : 0 : 1;
                                fieldViewModel.HideLabel = num != 0;
                                fieldViewModel1.ShowIndicator = form.FieldIndicationType != FormFieldIndication.NoIndicator && (field.Mandatory && form.FieldIndicationType == FormFieldIndication.MarkMandatoryFields || !field.Mandatory && form.FieldIndicationType == FormFieldIndication.MarkOptionalFields);
                                fieldViewModel1.Mandatory = field.Mandatory;
                                fieldViewModel1.Validate = !string.IsNullOrEmpty(field.RegEx);
                                fieldViewModel1.Regex = field.RegEx ?? string.Empty;
                                FieldViewModel? fieldViewModel2 = fieldViewModel1;
                                fieldViewModel2.PreValues = await FetchPrevalues(field, form, fieldPreValueSourceService, fieldPreValueSourceTypeService, appCaches, placeholderParsingService, additionalData);
                                fieldViewModel1.ToolTip = placeholderParsingService.ParsePlaceHolders(field.ToolTip ?? string.Empty, false, record, form, additionalData: additionalData);
                                fieldViewModel1.HasCondition = field.Condition is not null && field.Condition.Enabled;
                                fieldViewModel1.PlaceholderText = !field.Settings.TryGetValue("Placeholder", out string? str7) || string.IsNullOrEmpty(str7) ? string.Empty : placeholderParsingService.ParsePlaceHolders(str7, false, record, form, additionalData: additionalData);
                                fieldViewModel1.Condition = field.Condition;
                                fieldViewModel1.ConditionActionType = field.Condition is not null ? field.Condition.ActionType : FieldConditionActionType.Show;
                                fieldViewModel1.ConditionLogicType = field.Condition is not null ? field.Condition.LogicType : FieldConditionLogicType.Any;
                                fieldViewModel1.ConditionRules = field.Condition is not null ? [.. field.Condition.Rules] : [];
                                fieldViewModel1.CssClass = GetFieldCssClass(field, fieldType, fldCount);
                                fieldViewModel1.AdditionalSettings = field.Settings.ParseSettingsPlaceholders(placeholderParsingService, fieldType?.Settings() ?? new Dictionary<string, SettingAttribute>(StringComparer.OrdinalIgnoreCase), additionalData: additionalData);
                                fieldViewModel1.Values = field.Values;
                                FieldViewModel model = fieldViewModel1;
                                fieldViewModel2 = null;
                                fieldViewModel1 = null;
                                model.SetFileUploadOptions(field);
                                if (fieldType is not null)
                                {
                                    static string themeAccessor() => string.Empty;

                                    pageModel.RegisterFieldJavascriptAssets(themeAccessor, field, fieldType, hostingEnvironment);
                                    fieldsetContainerModel.Fields.Add(model);
                                }
                                ++fldCount;
                                fieldType = null;
                                field = null;
                            }
                        }
                        fieldsetModel.Containers.Add(fieldsetContainerModel);
                        fieldsetContainerModel = null;
                    }
                    pageModel.Fieldsets.Add(fieldsetModel);
                    fieldsetModel = null;
                }
                formViewModel.Pages.Add(pageModel);
                pageModel = null;
            }
            foreach (ValidationRule validationRule in form.ValidationRules)
            {
                ValidationRuleViewModel validationRuleViewModel = new()
                {
                    ErrorMessage = validationRule.ErrorMessage,
                    FieldId = validationRule.FieldId,
                    Rule = ReplaceFieldAliasWithIds(validationRule.Rule, form.AllFields)
                };
                formViewModel.ValidationRules.Add(validationRuleViewModel);
            }
        }

        private static string ReplaceFieldAliasWithIds(string rule, IEnumerable<Field> fields)
        {
            StringBuilder stringBuilder = new(rule);
            foreach (Field field in fields)
                stringBuilder.Replace("{" + field.Alias + "}", "{" + field.Id.ToString() + "}");
            return stringBuilder.ToString();
        }

        internal static string GetFieldCssClass(Field field, FieldType? fieldType, int fldCount)
        {
            HashSet<string> values =
              [
                "umbraco-forms-field",
                XmlHelper.XmlName(field.Alias)
              ];
            if (fieldType is not null)
                values.Add(XmlHelper.XmlName(fieldType.Name));
            if (field.Mandatory)
                values.Add("mandatory");
            if (fldCount % 2 == 0)
                values.Add("alternating");
            return string.Join(" ", values);
        }

        private static async Task<IEnumerable<PrevalueViewModel>> FetchPrevalues(
            Field field,
            Form form,
            IFieldPrevalueSourceService fieldPreValueSourceService,
            IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
            AppCaches appCaches,
            IPlaceholderParsingService placeholderParsingService,
            IDictionary<string, string?>? additionalData)
        {
            if (field.PreValues is not null && field.PreValues.Any())
                return field.PreValues.Select(pv => CreatePrevalueViewModelFromFormValues(form, pv, placeholderParsingService, additionalData));

            var preValueSource = fieldPreValueSourceService.GetById(field.PreValueSourceId);
            if (preValueSource is null)
                return [];

            var preValueSourceType = fieldPreValueSourceTypeService.GetById(preValueSource.FieldPrevalueSourceTypeId);
            if (preValueSourceType is null)
                return [];

            List<Prevalue>? source;
            if (preValueSource.CachePrevaluesFor == TimeSpan.Zero)
            {
                preValueSourceType.LoadSettings(preValueSource);
                source = await preValueSourceType.GetPreValuesAsync(field, form);
            }
            else
            {
                var cacheKey = $"Forms.PreValues.{field.PreValueSourceId}.{form.Id}.{field.Id}.{CultureInfo.CurrentUICulture.Name}";
                TimeSpan? cacheTime;
                if (preValueSource.CachePrevaluesFor.TotalMilliseconds == -1)
                {
                    cacheTime = null;
                }
                else
                {
                    cacheTime = preValueSource.CachePrevaluesFor;
                }

                source = await appCaches.RuntimeCache.GetCacheItemAsync(cacheKey, async () =>
                {
                    preValueSourceType.LoadSettings(preValueSource);
                    return await preValueSourceType.GetPreValuesAsync(field, form);
                }, cacheTime);
            }

            return source?.Select(pv => CreatePrevalueViewModelFromPrevalueSource(form, pv, placeholderParsingService, additionalData))
                ?? [];
        }

        private static PrevalueViewModel CreatePrevalueViewModelFromFormValues(
          Form form,
          FieldPrevalue pv,
          IPlaceholderParsingService placeholderParsingService,
          IDictionary<string, string?>? additionalData)
        {
            return new PrevalueViewModel()
            {
                Id = "0",
                Value = placeholderParsingService.ParsePlaceHolders(pv.Value, false, form: form, additionalData: additionalData),
                Caption = placeholderParsingService.ParsePlaceHolders(!string.IsNullOrWhiteSpace(pv.Caption) ? pv.Caption : pv.Value, false, form: form, additionalData: additionalData)
            };
        }

        private static PrevalueViewModel CreatePrevalueViewModelFromPrevalueSource(
          Form form,
          Prevalue pv,
          IPlaceholderParsingService placeholderParsingService,
          IDictionary<string, string?>? additionalData)
        {
            return new PrevalueViewModel()
            {
                Id = pv.Id.ToString(),
                Value = placeholderParsingService.ParsePlaceHolders(pv.Value, false, form: form, additionalData: additionalData),
                Caption = placeholderParsingService.ParsePlaceHolders(!string.IsNullOrWhiteSpace(pv.Caption) ? pv.Caption : pv.Value, false, form: form, additionalData: additionalData)
            };
        }
    }
}