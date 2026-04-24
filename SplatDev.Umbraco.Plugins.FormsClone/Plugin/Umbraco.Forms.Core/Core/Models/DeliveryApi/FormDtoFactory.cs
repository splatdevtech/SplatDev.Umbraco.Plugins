
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormDtoFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Helpers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormDtoFactory : DtoFactoryBase
  {
    private readonly IPlaceholderParsingService _placeholderParsingService;
    private readonly IFieldTypeStorage _fieldTypeStorage;
    private readonly IFieldPreValueSourceService _fieldPreValueSourceService;
    private readonly IFieldPreValueSourceTypeService _fieldPreValueSourceTypeService;
    private readonly IApiRichTextMarkupParser _apiRichTextMarkupParser;

    public FormDtoFactory(
      IPlaceholderParsingService placeholderParsingService,
      IEntityService entityService,
      IFieldTypeStorage fieldTypeStorage,
      IFieldPreValueSourceService fieldPreValueSourceService,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
      IPublishedContentCache publishedContentCache,
      IApiContentRouteBuilder apiContentRouteBuilder,
      IApiRichTextMarkupParser apiRichTextMarkupParser)
      : base(entityService, publishedContentCache, apiContentRouteBuilder)
    {
      this._placeholderParsingService = placeholderParsingService;
      this._fieldTypeStorage = fieldTypeStorage;
      this._fieldPreValueSourceService = fieldPreValueSourceService;
      this._fieldPreValueSourceTypeService = fieldPreValueSourceTypeService;
      this._apiRichTextMarkupParser = apiRichTextMarkupParser;
    }

    public FormDto BuildFormDefinitionDto(
      Form form,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      FormDto dto = new FormDto()
      {
        CssClass = form.CssClass,
        DisableDefaultStylesheet = form.DisableDefaultStylesheet,
        FieldIndicationType = form.FieldIndicationType,
        HideFieldValidation = form.HideFieldValidation,
        Id = form.Id,
        Indicator = this._placeholderParsingService.ParsePlaceHolders(form.Indicator, false, form: form, pageElements: pageElements, additionalData: additionalData),
        MessageOnSubmit = this._placeholderParsingService.ParsePlaceHolders(form.MessageOnSubmit ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
        MessageOnSubmitIsHtml = form.MessageOnSubmitIsHtml,
        Name = form.Name,
        Pages = (IEnumerable<FormPageDto>) form.Pages.Select<Page, FormPageDto>((Func<Page, FormPageDto>) (x => this.BuildFormPageDto(form, pageElements, additionalData, x))).ToList<FormPageDto>(),
        ShowValidationSummary = form.ShowValidationSummary,
        ValidationRules = (IEnumerable<FormValidationRuleDto>) form.ValidationRules.Select<ValidationRule, FormValidationRuleDto>((Func<ValidationRule, FormValidationRuleDto>) (x => this.BuildFormValidationRuleDto(form, pageElements, additionalData, x))).ToList<FormValidationRuleDto>()
      };
      dto.SubmitLabel = string.IsNullOrWhiteSpace(form.SubmitLabel) ? "Submit" : this._placeholderParsingService.ParsePlaceHolders(form.SubmitLabel, false, form: form, pageElements: pageElements, additionalData: additionalData);
      dto.PreviousLabel = string.IsNullOrWhiteSpace(form.PrevLabel) ? "Previous" : this._placeholderParsingService.ParsePlaceHolders(form.PrevLabel, false, form: form, pageElements: pageElements, additionalData: additionalData);
      dto.NextLabel = string.IsNullOrWhiteSpace(form.NextLabel) ? "Next" : this._placeholderParsingService.ParsePlaceHolders(form.NextLabel, false, form: form, pageElements: pageElements, additionalData: additionalData);
      this.PopulateGotoPageOnSubmit(form, (IPostSubmissionDetail) dto);
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
        Caption = this._placeholderParsingService.ParsePlaceHolders(page.Caption ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
        Condition = this.BuildConditionDto(page.ButtonCondition),
        Fieldsets = (IEnumerable<FormFieldsetDto>) page.FieldSets.Select<FieldSet, FormFieldsetDto>((Func<FieldSet, FormFieldsetDto>) (x => this.BuildFormFieldSetDto(form, pageElements, additionalData, x))).ToList<FormFieldsetDto>()
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
        Caption = this._placeholderParsingService.ParsePlaceHolders(fieldSet.Caption ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
        Columns = (IEnumerable<FormFieldsetColumnDto>) fieldSet.Containers.Select<FieldsetContainer, FormFieldsetColumnDto>((Func<FieldsetContainer, FormFieldsetColumnDto>) (x => this.BuildFormColumnDto(form, pageElements, additionalData, x))).ToList<FormFieldsetColumnDto>(),
        Condition = this.BuildConditionDto(fieldSet.Condition),
        Id = fieldSet.Id
      };
    }

    private FormFieldsetColumnDto BuildFormColumnDto(
      Form form,
      Hashtable? pageElements,
      IDictionary<string, string?>? additionalData,
      FieldsetContainer container)
    {
      return new FormFieldsetColumnDto()
      {
        Caption = this._placeholderParsingService.ParsePlaceHolders(container.Caption ?? string.Empty, false, form: form, pageElements: pageElements),
        Width = container.Width,
        Fields = (IEnumerable<FormFieldDto>) container.Fields.Select<Field, FormFieldDto>((Func<Field, FormFieldDto>) (x => this.BuildFormFieldDto(form, pageElements, additionalData, x))).Where<FormFieldDto>((Func<FormFieldDto, bool>) (x => x != null)).Select<FormFieldDto, FormFieldDto>((Func<FormFieldDto, FormFieldDto>) (x => x)).ToList<FormFieldDto>()
      };
    }

    private FormFieldDto? BuildFormFieldDto(
      Form form,
      Hashtable? pageElements,
      IDictionary<string, string?>? additionalData,
      Field field)
    {
      Umbraco.Forms.Core.FieldType fieldTypeByField = this._fieldTypeStorage.GetFieldTypeByField(field);
      if (fieldTypeByField == null)
        return (FormFieldDto) null;
      return new FormFieldDto()
      {
        Alias = field.Alias,
        FileUploadOptions = fieldTypeByField.SupportsUploadTypes ? this.BuildFormFileUploadOptionsDto(field) : (FormFileUploadOptionsDto) null,
        Caption = this._placeholderParsingService.ParsePlaceHolders(field.Caption, false, form: form, pageElements: pageElements, additionalData: additionalData),
        Condition = this.BuildConditionDto(field.Condition),
        CssClass = field.CssClass,
        HelpText = this._placeholderParsingService.ParsePlaceHolders(field.ToolTip ?? string.Empty, false, form: form, pageElements: pageElements, additionalData: additionalData),
        Id = field.Id,
        PreValues = this.PopulatePrevalues(form, field),
        Pattern = field.RegEx ?? string.Empty,
        Required = field.Mandatory,
        RequiredErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, this._placeholderParsingService, form, additionalData),
        PatternInvalidErrorMessage = FormRenderingHelper.FormatValidationErrorMessage(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, this._placeholderParsingService, form, additionalData),
        Settings = this.GetFieldSettings(field, fieldTypeByField, pageElements, additionalData),
        Type = this.BuildFormFieldTypeDto(fieldTypeByField)
      };
    }

    private IDictionary<string, string> GetFieldSettings(
      Field field,
      Umbraco.Forms.Core.FieldType fieldType,
      Hashtable? pageElements,
      IDictionary<string, string?>? additionalData)
    {
      Dictionary<string, SettingAttribute> dictionary = fieldType.Settings();
      IDictionary<string, string> settingsPlaceholders = field.Settings.ParseSettingsPlaceholders(this._placeholderParsingService, dictionary, pageElements: pageElements, additionalData: additionalData);
      this.ParseHtmlSettingsForApiResponse(dictionary, settingsPlaceholders);
      return settingsPlaceholders;
    }

    private void ParseHtmlSettingsForApiResponse(
      Dictionary<string, SettingAttribute> fieldTypeSettings,
      IDictionary<string, string> settings)
    {
      foreach (KeyValuePair<string, string> setting in (IEnumerable<KeyValuePair<string, string>>) settings)
      {
        SettingAttribute settingAttribute;
        if (fieldTypeSettings.TryGetValue(setting.Key, out settingAttribute) && settingAttribute.SupportsHtml)
          settings[setting.Key] = this._apiRichTextMarkupParser.Parse(settings[setting.Key]);
      }
    }

    private IEnumerable<FormFieldPrevalueDto> PopulatePrevalues(
      Form form,
      Field field)
    {
      FormRenderingHelper.EnsurePrevalues(form, field, this._fieldPreValueSourceService, this._fieldPreValueSourceTypeService);
      return (IEnumerable<FormFieldPrevalueDto>) field.PreValues.Select<FieldPrevalue, FormFieldPrevalueDto>(new Func<FieldPrevalue, FormFieldPrevalueDto>(this.BuildFormFieldPrevalue)).ToList<FormFieldPrevalueDto>();
    }

    private FormConditionDto? BuildConditionDto(FieldCondition? condition)
    {
      if (condition == null || !condition.Enabled)
        return (FormConditionDto) null;
      return new FormConditionDto()
      {
        ActionType = condition.ActionType,
        LogicType = condition.LogicType,
        Rules = (IEnumerable<FormConditionRuleDto>) condition.Rules.Select<FieldConditionRule, FormConditionRuleDto>(new Func<FieldConditionRule, FormConditionRuleDto>(this.BuildFormConditionRuleDto)).ToList<FormConditionRuleDto>()
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

    private FormFieldPrevalueDto BuildFormFieldPrevalue(FieldPrevalue prevalue) => new FormFieldPrevalueDto()
    {
      Caption = prevalue.Caption,
      Value = prevalue.Value
    };

    private FormFieldTypeDto BuildFormFieldTypeDto(Umbraco.Forms.Core.FieldType fieldType) => new FormFieldTypeDto()
    {
      Id = fieldType.Id,
      Name = fieldType.Name,
      SupportsPreValues = fieldType.SupportsPreValues,
      SupportsUploadTypes = fieldType.SupportsUploadTypes,
      RenderInputType = fieldType.RenderInputType.ToString()
    };

    private FormFileUploadOptionsDto BuildFormFileUploadOptionsDto(
      Field field)
    {
      FormFileUploadOptionsDto model = new FormFileUploadOptionsDto();
      model.SetFileUploadOptions(field);
      return model;
    }

    private FormValidationRuleDto BuildFormValidationRuleDto(
      Form form,
      Hashtable? pageElements,
      IDictionary<string, string?>? additionalData,
      ValidationRule validationRule)
    {
      return new FormValidationRuleDto()
      {
        ErrorMessage = this._placeholderParsingService.ParsePlaceHolders(validationRule.ErrorMessage, false, form: form, pageElements: pageElements, additionalData: additionalData),
        Rule = validationRule.Rule,
        FieldId = validationRule.FieldId
      };
    }
  }
}
