using FormBuilder.Core.Attributes;
using FormBuilder.Core.Dto;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using System.Collections;

using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Factory
{
    public class FormDtoFactory(
      IPlaceholderParsingService placeholderParsingService,
      IEntityService entityService,
      IFieldTypeStorage fieldTypeStorage,
      IFieldPrevalueSourceService fieldPreValueSourceService,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder,
      IApiRichTextMarkupParser apiRichTextMarkupParser) : DtoFactoryBase(entityService, publishedContentCache, apiContentRouteBuilder)
    {
        private readonly IPlaceholderParsingService _placeholderParsingService = placeholderParsingService;
        private readonly IFieldTypeStorage _fieldTypeStorage = fieldTypeStorage;
        private readonly IFieldPrevalueSourceService _fieldPreValueSourceService = fieldPreValueSourceService;
        private readonly IFieldPrevalueSourceTypeService _fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
        private readonly IApiRichTextMarkupParser _apiRichTextMarkupParser = apiRichTextMarkupParser;

        public FormDto BuildFormDefinitionDto(
          Form form,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null)
        {
            FormDto dto = new()
            {
                CssClass = form.CssClass,
                DisableDefaultStylesheet = form.DisableDefaultStylesheet,
                FieldIndicationType = form.FieldIndicationType,
                HideFieldValidation = form.HideFieldValidation,
                Id = form.Id,
                Indicator = _placeholderParsingService.ParsePlaceHolders(form.Indicator, false, form: form, pageElements: pageElements, additionalData: additionalData),
                MessageOnSubmit = _placeholderParsingService.ParsePlaceHolders(form.MessageOnSubmit ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
                MessageOnSubmitIsHtml = form.MessageOnSubmitIsHtml,
                Name = form.Name,
                Pages = [.. form.Pages.Select(x => BuildFormPageDto(form, pageElements, additionalData, x))],
                ShowValidationSummary = form.ShowValidationSummary,
                ValidationRules = [.. form.ValidationRules.Select(x => BuildFormValidationRuleDto(form, pageElements, additionalData, x))],
                SubmitLabel = string.IsNullOrWhiteSpace(form.SubmitLabel) ? "Submit" : _placeholderParsingService.ParsePlaceHolders(form.SubmitLabel, false, form: form, pageElements: pageElements, additionalData: additionalData),
                PreviousLabel = string.IsNullOrWhiteSpace(form.PrevLabel) ? "Previous" : _placeholderParsingService.ParsePlaceHolders(form.PrevLabel, false, form: form, pageElements: pageElements, additionalData: additionalData),
                NextLabel = string.IsNullOrWhiteSpace(form.NextLabel) ? "Next" : _placeholderParsingService.ParsePlaceHolders(form.NextLabel, false, form: form, pageElements: pageElements, additionalData: additionalData)
            };
            PopulateGotoPageOnSubmit(form, dto);
            return dto;
        }

        private FormPageDto BuildFormPageDto(
          Form form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          Page page)
        {
            return new FormPageDto()
            {
                Caption = _placeholderParsingService.ParsePlaceHolders(page.Caption ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
                Condition = BuildConditionDto(page.ButtonCondition),
                Fieldsets = [.. page.FieldSets.Select(x => BuildFormFieldSetDto(form, pageElements, additionalData, x))]
            };
        }

        private FormFieldsetDto BuildFormFieldSetDto(
          Form form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          FieldSet fieldSet)
        {
            return new FormFieldsetDto()
            {
                Caption = _placeholderParsingService.ParsePlaceHolders(fieldSet.Caption ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
                Columns = [.. fieldSet.Containers.Select(x => BuildFormColumnDto(form, pageElements, additionalData, x))],
                Condition = BuildConditionDto(fieldSet.Condition),
                Id = fieldSet.Id
            };
        }

        private FormFieldsetColumnDto BuildFormColumnDto(
          Form form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          FieldsetContainer container)
        {
            var fields = container.Fields.Select(x => BuildFormFieldDto(form, pageElements, additionalData, x));
            var filteredFields = fields.Where(x => x is not null).Select(x => x);
            return new FormFieldsetColumnDto()
            {
                Caption = _placeholderParsingService.ParsePlaceHolders(container.Caption ?? string.Empty, false, form: form, pageElements: pageElements),
                Width = container.Width,
                Fields = filteredFields is not null ? [.. filteredFields!] : []
            };
        }

        private FormFieldDto? BuildFormFieldDto(
          Form form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          Field field)
        {
            FieldTypes.FieldType? fieldTypeByField = _fieldTypeStorage.GetFieldTypeByField(field);
            if (fieldTypeByField is null)
                return null;
            return new FormFieldDto()
            {
                Alias = field.Alias,
                FileUploadOptions = fieldTypeByField.SupportsUploadTypes ? BuildFormFileUploadOptionsDto(field) : null,
                Caption = _placeholderParsingService.ParsePlaceHolders(field.Caption, false, form: form, pageElements: pageElements, additionalData: additionalData),
                Condition = BuildConditionDto(field.Condition),
                CssClass = field.CssClass,
                HelpText = _placeholderParsingService.ParsePlaceHolders(field.ToolTip ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
                Id = field.Id,
                PreValues = PopulatePrevalues(form, field),
                Pattern = field.RegEx ?? string.Empty,
                Required = field.Mandatory,
                RequiredErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, _placeholderParsingService, form, additionalData),
                PatternInvalidErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, _placeholderParsingService, form, additionalData),
                Settings = GetFieldSettings(field, fieldTypeByField, pageElements, additionalData),
                Type = BuildFormFieldTypeDto(fieldTypeByField)
            };
        }

        private IDictionary<string, string> GetFieldSettings(
          Field field,
          FieldTypes.FieldType fieldType,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData)
        {
            Dictionary<string, SettingAttribute> dictionary = fieldType.Settings();
            IDictionary<string, string> settingsPlaceholders = field.Settings.ParseSettingsPlaceholders(_placeholderParsingService, dictionary, pageElements: pageElements, additionalData: additionalData);
            ParseHtmlSettingsForApiResponse(dictionary, settingsPlaceholders);
            return settingsPlaceholders;
        }

        private void ParseHtmlSettingsForApiResponse(
          Dictionary<string, SettingAttribute> fieldTypeSettings,
          IDictionary<string, string> settings)
        {
            foreach (KeyValuePair<string, string> setting in (IEnumerable<KeyValuePair<string, string>>)settings)
            {
                if (fieldTypeSettings.TryGetValue(setting.Key, out SettingAttribute? settingAttribute) && settingAttribute.SupportsHtml)
                    settings[setting.Key] = _apiRichTextMarkupParser.Parse(settings[setting.Key]);
            }
        }

        private IEnumerable<FormFieldPrevalueDto> PopulatePrevalues(
          Form? form,
          Field field)
        {
            using var _ = FormRenderingHelper.EnsurePrevalues(form, field, _fieldPreValueSourceService, _fieldPreValueSourceTypeService);
            return [.. field.PreValues.Select(new Func<FieldPrevalue, FormFieldPrevalueDto>(BuildFormFieldPrevalue))];
        }

        private FormConditionDto? BuildConditionDto(FieldCondition? condition)
        {
            if (condition is null || !condition.Enabled)
                return null;
            return new FormConditionDto()
            {
                ActionType = condition.ActionType,
                LogicType = condition.LogicType,
                Rules = [.. condition.Rules.Select(new Func<FieldConditionRule, FormConditionRuleDto>(BuildFormConditionRuleDto))]
            };
        }

        private FormConditionRuleDto BuildFormConditionRuleDto(
          FieldConditionRule rule)
        {
            return new FormConditionRuleDto()
            {
                Field = rule.Field.ToString(),
                Operator = rule.Operator,
                Value = rule.Value
            };
        }

        private FormFieldPrevalueDto BuildFormFieldPrevalue(FieldPrevalue prevalue) => new()
        {
            Caption = prevalue.Caption,
            Value = prevalue.Value
        };

        private static FormFieldTypeDto BuildFormFieldTypeDto(FieldTypes.FieldType fieldType)
        {
            return new()
            {
                Id = fieldType.Id,
                Name = fieldType.Name,
                SupportsPreValues = fieldType.SupportsPreValues,
                SupportsUploadTypes = fieldType.SupportsUploadTypes,
                RenderInputType = fieldType.RenderInputType.ToString()
            };
        }

        private static FormFileUploadOptionsDto BuildFormFileUploadOptionsDto(
          Field field)
        {
            FormFileUploadOptionsDto model = new();
            model.SetFileUploadOptions(field);
            return model;
        }

        private FormValidationRuleDto BuildFormValidationRuleDto(
          Form? form,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          ValidationRule validationRule)
        {
            return new FormValidationRuleDto()
            {
                ErrorMessage = _placeholderParsingService.ParsePlaceHolders(validationRule.ErrorMessage, false, form: form, pageElements: pageElements, additionalData: additionalData),
                Rule = validationRule.Rule,
                FieldId = validationRule.FieldId
            };
        }
    }
}