
// Type: Umbraco.Forms.Web.Models.FormViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Html;

using System.Globalization;
using System.Text;
using System.Web;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Templates;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Helpers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
    [Serializable]
    public class FormViewModel
    {
        public Guid FormId { get; set; }

        public string FormClientId { get; set; } = string.Empty;

        public string FormName { get; set; } = string.Empty;

        public List<PageViewModel> Pages { get; set; } = new List<PageViewModel>();

        public List<ValidationRuleViewModel> ValidationRules { get; set; } = new List<ValidationRuleViewModel>();

        public int FormStep { get; set; }

        public Guid RecordId { get; set; }

        public string? Theme { get; set; }

        public bool RenderScriptFiles { get; set; }

        public Dictionary<string, object[]> FormState { get; set; } = new Dictionary<string, object[]>();

        public bool PageHandled { get; set; }

        public bool SubmitHandled { get; set; }

        public string Indicator { get; set; } = string.Empty;

        public bool ShowFieldValidaton { get; set; }

        public bool ShowValidationSummary { get; set; }

        public string MessageOnSubmit { get; set; } = string.Empty;

        public bool MessageOnSubmitIsHtml { get; set; }

        public bool DisableDefaultStylesheet { get; set; }

        public string? PreviousClicked { get; set; }

        public string RecordState { get; set; } = string.Empty;

        public bool RenderAntiForgeryToken { get; set; }

        public string? AdditionalData { get; set; }

        public PageViewModel? CurrentPage => this.Pages.Count<PageViewModel>() >= this.FormStep + 1 ? this.Pages[this.FormStep] : null;

        public bool IsMultiPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public int PageNumber => this.FormStep + 1;

        public int PageCount => this.Pages.Count<PageViewModel>() + (this.HasSummaryPage ? 1 : 0);

        public string CssClass { get; set; } = string.Empty;

        public string SubmitCaption { get; set; } = string.Empty;

        public string NextCaption { get; set; } = string.Empty;

        public string PreviousCaption { get; set; } = string.Empty;

        public bool ShowPagingOnMultiPageFormsAtTop { get; set; }

        public bool ShowPagingOnMultiPageFormsAtBottom { get; set; }

        public string PagingDetailsFormat { get; set; } = "Page {0} of {1}";

        public string PageCaptionFormat { get; set; } = "Page {0}";

        public bool HasSummaryPage { get; set; }

        public bool ShowSummaryPage { get; set; }

        public string SummaryCaption { get; set; } = string.Empty;

        public Dictionary<Guid, ConditionViewModel> PageButtonConditions { get; set; } = new Dictionary<Guid, ConditionViewModel>();

        public Dictionary<Guid, ConditionViewModel> FieldsetConditions { get; set; } = new Dictionary<Guid, ConditionViewModel>();

        public Dictionary<Guid, ConditionViewModel> FieldConditions { get; set; } = new Dictionary<Guid, ConditionViewModel>();

        public string TriggerConditionsCheckOn { get; set; } = "change";

        public string FormElementHtmlIdPrefix { get; set; } = string.Empty;

        public IDictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();

        public IDictionary<string, string> JavaScriptTagAttributes { get; set; } = new Dictionary<string, string>()
    {
      {
        "defer",
        "defer"
      }
    };

        public bool DisableClientSideValidationDependencyCheck { get; set; }

        public Guid? RedirectToPageId { get; set; }

        public IDictionary<string, string> GetFieldsNotDisplayed(
          string formElementHtmlIdPrefix)
        {
            return this.Pages.Select((x, index) => new
            {
                item = x,
                index = index
            }).Where(x => x.index != this.FormStep).Select(x => x.item).SelectMany<PageViewModel, FieldsetViewModel>(x => x.Fieldsets).SelectMany<FieldsetViewModel, FieldsetContainerViewModel>(x => x.Containers).SelectMany<FieldsetContainerViewModel, FieldViewModel>(x => x.Fields).ToDictionary<FieldViewModel, string, string>(f => formElementHtmlIdPrefix + f.Name, f => f.Values == null ? string.Empty : string.Join<object>(";;", f.Values));
        }

        public IHtmlContent GetMessageOnSubmit() => new HtmlString(this.MessageOnSubmitIsHtml ? this.MessageOnSubmit : HttpUtility.HtmlEncode(this.MessageOnSubmit));

        public string GetPageCaption(int index)
        {
            if (index >= this.Pages.Count)
                return string.Empty;
            return !string.IsNullOrWhiteSpace(this.Pages[index].Caption) ? this.Pages[index].Caption : string.Format(this.PageCaptionFormat, index + 1);
        }

        public async Task Build(
          Form form,
          Record? record,
          IDictionary<string, string?>? additionalData,
          IFieldTypeStorage fieldTypeStorage,
          IFieldPreValueSourceService fieldPreValueSourceService,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
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
                PageViewModel pageModel = new PageViewModel()
                {
                    Id = page.Id,
                    Caption = placeholderParsingService.ParsePlaceHolders(page.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                    HasButtonCondition = page.ButtonCondition != null && page.ButtonCondition.Enabled,
                    ButtonCondition = page.ButtonCondition,
                    ButtonConditionActionType = page.ButtonCondition != null ? page.ButtonCondition.ActionType : FieldConditionActionType.Show,
                    ButtonConditionLogicType = page.ButtonCondition != null ? page.ButtonCondition.LogicType : FieldConditionLogicType.Any,
                    ButtonConditionRules = page.ButtonCondition != null ? page.ButtonCondition.Rules.ToList<FieldConditionRule>() : (IEnumerable<FieldConditionRule>)new List<FieldConditionRule>()
                };
                if (page.ButtonCondition != null && page.ButtonCondition.Enabled)
                    formViewModel.PageButtonConditions.Add(page.Id, ConditionViewModel.Build(form, page.ButtonCondition, placeholderParsingService));
                if (!form.DisableDefaultStylesheet)
                    pageModel.CssFiles.Add("default", "~/App_Plugins/UmbracoForms/assets/defaultform.min.css");
                foreach (FieldSet fieldset in page.FieldSets)
                {
                    FieldsetViewModel fieldsetModel = new FieldsetViewModel()
                    {
                        Id = formDesignSettings.FormElementHtmlIdPrefix + fieldset.Id.ToString(),
                        Caption = placeholderParsingService.ParsePlaceHolders(fieldset.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                        HasCondition = fieldset.Condition != null && fieldset.Condition.Enabled,
                        Condition = fieldset.Condition,
                        ConditionActionType = fieldset.Condition != null ? fieldset.Condition.ActionType : FieldConditionActionType.Show,
                        ConditionLogicType = fieldset.Condition != null ? fieldset.Condition.LogicType : FieldConditionLogicType.Any,
                        ConditionRules = fieldset.Condition != null ? fieldset.Condition.Rules.ToList<FieldConditionRule>() : (IEnumerable<FieldConditionRule>)new List<FieldConditionRule>()
                    };
                    if (fieldset.Condition != null && fieldset.Condition.Enabled)
                        formViewModel.FieldsetConditions.Add(fieldset.Id, ConditionViewModel.Build(form, fieldset.Condition, placeholderParsingService));
                    foreach (FieldsetContainer container in fieldset.Containers)
                    {
                        FieldsetContainerViewModel fieldsetContainerModel = new FieldsetContainerViewModel()
                        {
                            Caption = placeholderParsingService.ParsePlaceHolders(container.Caption ?? string.Empty, false, record, form, additionalData: additionalData),
                            Width = container.Width
                        };
                        foreach (Field field1 in container.Fields)
                        {
                            Field field = field1;
                            FieldType fieldType = fieldTypeStorage.GetFieldTypeByField(field);
                            if (fieldType != null)
                            {
                                if (field.Condition != null && field.Condition.Enabled)
                                    formViewModel.FieldConditions.Add(field.Id, ConditionViewModel.Build(form, field.Condition, placeholderParsingService));
                                field.PopulateDefaultValue(placeholderParsingService, additionalData);
                                string placeHolders = placeholderParsingService.ParsePlaceHolders(field.Caption, false, record, form, additionalData: additionalData);
                                FieldViewModel fieldViewModel1 = new FieldViewModel();
                                fieldViewModel1.Alias = field.Alias;
                                fieldViewModel1.Id = formDesignSettings.FormElementHtmlIdPrefix + field.Id.ToString();
                                fieldViewModel1.FieldsetId = fieldset.Id;
                                fieldViewModel1.PageId = page.Id;
                                fieldViewModel1.FormId = form.Id;
                                fieldViewModel1.Name = field.Id.ToString();
                                fieldViewModel1.Caption = placeHolders;
                                fieldViewModel1.RequiredErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, placeholderParsingService, form, additionalData);
                                fieldViewModel1.InvalidErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, placeholderParsingService, form, additionalData);
                                fieldViewModel1.FieldTypeName = fieldType?.Name ?? string.Empty;
                                fieldViewModel1.FieldType = fieldType;
                                FieldViewModel fieldViewModel = fieldViewModel1;
                                FieldType fieldType1 = fieldType;
                                int num = fieldType1 != null ? (fieldType1.HideLabel ? 1 : 0) : 1;
                                fieldViewModel.HideLabel = num != 0;
                                fieldViewModel1.ShowIndicator = form.FieldIndicationType != FormFieldIndication.NoIndicator && (field.Mandatory && form.FieldIndicationType == FormFieldIndication.MarkMandatoryFields || !field.Mandatory && form.FieldIndicationType == FormFieldIndication.MarkOptionalFields);
                                fieldViewModel1.Mandatory = field.Mandatory;
                                fieldViewModel1.Validate = !string.IsNullOrEmpty(field.RegEx);
                                fieldViewModel1.Regex = field.RegEx ?? string.Empty;
                                FieldViewModel fieldViewModel2 = fieldViewModel1;
                                fieldViewModel2.PreValues = await FormViewModel.FetchPrevalues(field, form, fieldPreValueSourceService, fieldPreValueSourceTypeService, appCaches, placeholderParsingService, additionalData);
                                fieldViewModel1.ToolTip = placeholderParsingService.ParsePlaceHolders(field.ToolTip ?? string.Empty, false, record, form, additionalData: additionalData);
                                fieldViewModel1.HasCondition = field.Condition != null && field.Condition.Enabled;
                                string str7;
                                fieldViewModel1.PlaceholderText = !field.Settings.TryGetValue("Placeholder", out str7) || string.IsNullOrEmpty(str7) ? string.Empty : placeholderParsingService.ParsePlaceHolders(str7, false, record, form, additionalData: additionalData);
                                fieldViewModel1.Condition = field.Condition;
                                fieldViewModel1.ConditionActionType = field.Condition != null ? field.Condition.ActionType : FieldConditionActionType.Show;
                                fieldViewModel1.ConditionLogicType = field.Condition != null ? field.Condition.LogicType : FieldConditionLogicType.Any;
                                fieldViewModel1.ConditionRules = field.Condition != null ? field.Condition.Rules.ToList<FieldConditionRule>() : (IEnumerable<FieldConditionRule>)new List<FieldConditionRule>();
                                fieldViewModel1.CssClass = FormViewModel.GetFieldCssClass(field, fieldType, fldCount);
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
                ValidationRuleViewModel validationRuleViewModel = new ValidationRuleViewModel()
                {
                    ErrorMessage = placeholderParsingService.ParsePlaceHolders(validationRule.ErrorMessage, false, record, form, additionalData: additionalData),
                    FieldId = validationRule.FieldId,
                    Rule = formViewModel.ReplaceFieldAliasWithIds(validationRule.Rule, form.AllFields)
                };
                formViewModel.ValidationRules.Add(validationRuleViewModel);
            }
        }

        private string ReplaceFieldAliasWithIds(string rule, IEnumerable<Field> fields)
        {
            StringBuilder stringBuilder = new StringBuilder(rule);
            foreach (Field field in fields)
                stringBuilder.Replace("{" + field.Alias + "}", "{" + field.Id.ToString() + "}");
            return stringBuilder.ToString();
        }

        internal static string GetFieldCssClass(Field field, FieldType? fieldType, int fldCount)
        {
            HashSet<string> values = new HashSet<string>()
      {
        "umbraco-forms-field",
        XmlHelper.XmlName(field.Alias)
      };
            if (fieldType != null)
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
            IFieldPreValueSourceService fieldPreValueSourceService,
            IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
            AppCaches appCaches,
            IPlaceholderParsingService placeholderParsingService,
            IDictionary<string, string?>? additionalData)
        {
            if (field.PreValues is not null && field.PreValues.Any())
                return field.PreValues.Select(pv => CreatePrevalueViewModelFromFormValues(form, pv, placeholderParsingService, additionalData));

            var preValueSource = fieldPreValueSourceService.GetById(field.PreValueSourceId);
            if (preValueSource is null)
                return [];

            var preValueSourceType = fieldPreValueSourceTypeService.GetById(preValueSource.FieldPreValueSourceTypeId);
            if (preValueSourceType is null)
                return [];

            List<PreValue>? source;
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
          PreValue pv,
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
