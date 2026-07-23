using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Evaluators;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Umbraco.Extensions;

namespace FormBuilder.Core.FieldTypes
{
    [DataContract]
    [Serializable]
    public abstract class FieldType : ProviderBase, IFieldType
    {
        private bool _supportsPrevalues;
        private bool _supportsRegex;
        private bool _supportsMandatory = true;
        private RenderInputType _renderInputType;
        private bool _hideLabel;
        private bool _rendered;
        private string _renderView = "text";
        private string _previewView = string.Empty;
        private string _editView = "Umb.PropertyEditorUi.Label";
        private string _fieldTypeViewName = string.Empty;

        public virtual bool SupportsPreValues
        {
            get => _supportsPrevalues;
            set => _supportsPrevalues = value;
        }

        public virtual FieldDataType DataType { get; set; }

        public virtual bool SupportsUploadTypes { get; set; }

        public virtual bool SupportsMandatory
        {
            get => _supportsMandatory;
            set => _supportsMandatory = value;
        }

        public virtual bool SupportsRegex
        {
            get => _supportsRegex;
            set => _supportsRegex = value;
        }

        public virtual RenderInputType RenderInputType
        {
            get => _renderInputType;
            set => _renderInputType = value;
        }

        public virtual bool StoresData => true;

        public virtual bool HideLabel
        {
            get => _hideLabel;
            set => _hideLabel = value;
        }

        public virtual bool Rendered
        {
            get => _rendered;
            set => _rendered = value;
        }

        public virtual string RenderView
        {
            get => _renderView;
            set => _renderView = value;
        }

        public virtual string PreviewView
        {
            get => _previewView;
            set => _previewView = value;
        }

        public virtual string EditView
        {
            get => _editView;
            set => _editView = value;
        }

        public virtual string FieldTypeViewName
        {
            get => string.IsNullOrEmpty(_fieldTypeViewName) ? string.Format("FieldType.{0}.cshtml", Name) : _fieldTypeViewName;
            set => _fieldTypeViewName = value;
        }

        public new virtual string Icon { get; set; } = string.Empty;

        public virtual string Category { get; set; } = string.Empty;

        public virtual int SortOrder { get; set; }

        public virtual Dictionary<FieldConditionRuleOperator, Func<string, string, bool>> ConditionCheckFunctions
        {
            get
            {
                return new Dictionary<FieldConditionRuleOperator, Func<string, string, bool>>()
        {
          {
            FieldConditionRuleOperator.Is,
             (comparedValue, fieldValue) => Is(comparedValue, fieldValue)
          },
          {
            FieldConditionRuleOperator.IsNot,
             (comparedValue, fieldValue) => !Is(comparedValue, fieldValue)
          },
          {
            FieldConditionRuleOperator.Contains,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.Contains(comparedValue)
          },
          {
            FieldConditionRuleOperator.ContainsIgnoreCase,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.Contains(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.StartsWith,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.StartsWith(comparedValue)
          },
          {
            FieldConditionRuleOperator.StartsWithIgnoreCase,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.StartsWith(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.EndsWith,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.EndsWith(comparedValue)
          },
          {
            FieldConditionRuleOperator.EndsWithIgnoreCase,
             (comparedValue, fieldValue) => comparedValue is not null && fieldValue is not null && fieldValue.EndsWith(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.GreaterThen,
             (comparedValue, fieldValue) =>
            {
              int num = double.TryParse(comparedValue, out double result1) ? 1 : 0;
              bool flag = DateTime.TryParse(comparedValue, out DateTime result2);
              if (num != 0)
              {
                if (double.TryParse(fieldValue, out double result3))
                  return result3 > result1;
                if (string.IsNullOrEmpty(fieldValue))
                  return 0.0 > result1;
              }
              else
              {
                if (flag && DateTime.TryParse(fieldValue, out DateTime result4))
                  return result4 > result2;
              }
              return string.Compare(fieldValue, comparedValue, StringComparison.Ordinal) > 0;
            }
          },
          {
            FieldConditionRuleOperator.LessThen,
             (comparedValue, fieldValue) =>
            {
              int num = double.TryParse(comparedValue, out double result5) ? 1 : 0;
              bool flag = DateTime.TryParse(comparedValue, out DateTime result6);
              if (num != 0)
              {
                if (double.TryParse(fieldValue, out double result7))
                  return result7 < result5;
                if (string.IsNullOrEmpty(fieldValue))
                  return 0.0 < result5;
              }
              else
              {
                if (flag && DateTime.TryParse(fieldValue, out DateTime result8))
                  return result8 < result6;
              }
              return string.Compare(fieldValue, comparedValue, StringComparison.Ordinal) < 0;
            }
          },
          {
            FieldConditionRuleOperator.NotContains,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.Contains(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotContainsIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.Contains(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          },
          {
            FieldConditionRuleOperator.NotStartsWith,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.StartsWith(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotStartsWithIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.StartsWith(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          },
          {
            FieldConditionRuleOperator.NotEndsWith,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.EndsWith(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotEndsWithIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue is null)
                return true;
              return comparedValue is not null && !fieldValue.EndsWith(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          }
        };

                static bool Is(string comparedValue, string fieldValue) => string.Equals(comparedValue, fieldValue);
            }
        }

        public virtual bool MandatoryByDefault { get; set; }

        public virtual IEnumerable<string> RequiredJavascriptFiles(Field field) => [];

        public virtual IEnumerable<string> RequiredPartialViews(Field field) => [];

        public virtual IEnumerable<string> RequiredPartialViews(
          Func<string?> themeAccessor,
          Field field)
        {
            return [];
        }

        public virtual IEnumerable<string> RequiredCssFiles(Field field) => [];

        public virtual string RequiredJavascriptInitialization(Field field) => string.Empty;

        public Type GetDataType()
        {
            return DataType switch
            {
                FieldDataType.String => typeof(string),
                FieldDataType.LongString => typeof(string),
                FieldDataType.Integer => typeof(int),
                FieldDataType.DateTime => typeof(DateTime),
                FieldDataType.Bit => typeof(bool),
                _ => typeof(string),
            };
        }

        public virtual string GetDesignView() => string.Format("~/App_Plugins/FormBuilder/backoffice/Common/FieldTypes/{0}.html", GetType().Name.ToLower());

        public virtual Dictionary<string, SettingAttribute> Settings()
        {
            Dictionary<string, SettingAttribute> dictionary = new(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SettingAttribute), true);
                if (customAttributes.Length != 0)
                    dictionary.Add(property.Name, (SettingAttribute)customAttributes[0]);
            }
            return dictionary;
        }

        public virtual IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage,
          List<string> errors)
        {
            string input = string.Join(",", postedValues.Where(x => !x.ToString().IsNullOrWhiteSpace()));
            if (!string.IsNullOrEmpty(field.RegEx) && !new Regex(field.RegEx).Match(input).Success && input != string.Empty)
                AddValidationError(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, placeholderParsingService, form, errors);
            if (field.Mandatory && string.IsNullOrEmpty(input) && IsFieldVisible(form, field, fieldTypeStorage, placeholderParsingService))
                AddValidationError(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, placeholderParsingService, form, errors);
            if (field.PreValues is not null && field.PreValues.Any())
            {
                HashSet<object> source = [.. field.PreValues.Select(x => placeholderParsingService.ParsePlaceHolders(x.Value, false, form: form))];
                foreach (object postedValue in postedValues)
                {
                    if (!source.Contains(postedValue) && !string.IsNullOrEmpty(postedValue.ToString()))
                        errors.Add(string.Format("Unexpected value: '{0}'. Expected one of these: {1}", postedValue, string.Join(", ", source.Select(x => string.Format("'{0}'", x)))));
                }
            }
            return errors;
        }

        protected static bool IsFieldVisible(
          Form form,
          Field field,
          IFieldTypeStorage fieldTypeStorage,
          IPlaceholderParsingService placeholderParsingService)
        {
            if (!IsFieldVisibleWithCondition(form, field, fieldTypeStorage, placeholderParsingService))
                return false;
            FieldSet conditionedElement = form.AllFieldSets.Single(x => x.Containers.SelectMany(y => y.Fields).Any(z => z.Id == field.Id));
            return IsFieldVisibleWithCondition(form, conditionedElement, fieldTypeStorage, placeholderParsingService);
        }

        private static bool IsFieldVisibleWithCondition(
        Form form,
        IConditioned conditionedElement,
        IFieldTypeStorage fieldTypeStorage,
        IPlaceholderParsingService placeholderParsingService)
        {
            if (!conditionedElement.HasCondition())
                return true;
            if (conditionedElement.Condition is not null && conditionedElement.Condition.IsCircular(form))
                return false;
            Dictionary<Guid, string> dictionary = form.AllFields.ToDictionary(f => f.Id, f => string.Join(", ", f.Values ?? []));
            return conditionedElement.Condition.IsVisible(form, fieldTypeStorage, dictionary, placeholderParsingService);
        }

        private static void AddValidationError(
          string? fieldMessage,
          string formMessage,
          string caption,
          IPlaceholderParsingService placeholderParsingService,
          Form form,
          List<string> errors)
        {
            string str = string.Format(string.IsNullOrWhiteSpace(fieldMessage) ? formMessage : fieldMessage, caption);
            errors.Add(placeholderParsingService.ParsePlaceHolders(str, false, form: form));
        }

        public virtual IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            return ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage, []);
        }

        public virtual IEnumerable<object> ProcessSubmittedValue(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            object[] array = [.. postedValues];
            return array.Length == 0 && field.Values is not null && field.Values.Count > 0 ? field.Values : array;
        }

        public virtual IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = [.. postedValues];
            return record;
        }

        public virtual IEnumerable<object> ConvertFromRecord(
          Field field,
          IEnumerable<object> storedValues)
        {
            List<object> objectList = [.. storedValues];
            return objectList;
        }

        public virtual List<Exception> ValidateSettings() => [];
    }
}