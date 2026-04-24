
// Type: Umbraco.Forms.Core.FieldType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;

using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Umbraco.Extensions;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core
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
            get => this._supportsPrevalues;
            set => this._supportsPrevalues = value;
        }

        public virtual FieldDataType DataType { get; set; }

        public virtual bool SupportsUploadTypes { get; set; }

        public virtual bool SupportsMandatory
        {
            get => this._supportsMandatory;
            set => this._supportsMandatory = value;
        }

        public virtual bool SupportsRegex
        {
            get => this._supportsRegex;
            set => this._supportsRegex = value;
        }

        public virtual RenderInputType RenderInputType
        {
            get => this._renderInputType;
            set => this._renderInputType = value;
        }

        public virtual bool StoresData => true;

        public virtual bool HideLabel
        {
            get => this._hideLabel;
            set => this._hideLabel = value;
        }

        public virtual bool Rendered
        {
            get => this._rendered;
            set => this._rendered = value;
        }

        public virtual string RenderView
        {
            get => this._renderView;
            set => this._renderView = value;
        }

        public virtual string PreviewView
        {
            get => this._previewView;
            set => this._previewView = value;
        }

        public virtual string EditView
        {
            get => this._editView;
            set => this._editView = value;
        }

        public virtual string FieldTypeViewName
        {
            get => string.IsNullOrEmpty(this._fieldTypeViewName) ? string.Format("FieldType.{0}.cshtml", Name) : this._fieldTypeViewName;
            set => this._fieldTypeViewName = value;
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
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.Contains(comparedValue)
          },
          {
            FieldConditionRuleOperator.ContainsIgnoreCase,
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.Contains(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.StartsWith,
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.StartsWith(comparedValue)
          },
          {
            FieldConditionRuleOperator.StartsWithIgnoreCase,
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.StartsWith(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.EndsWith,
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.EndsWith(comparedValue)
          },
          {
            FieldConditionRuleOperator.EndsWithIgnoreCase,
             (comparedValue, fieldValue) => comparedValue != null && fieldValue != null && fieldValue.EndsWith(comparedValue, StringComparison.OrdinalIgnoreCase)
          },
          {
            FieldConditionRuleOperator.GreaterThen,
             (comparedValue, fieldValue) =>
            {
              double result1;
              int num = double.TryParse(comparedValue, out result1) ? 1 : 0;
              DateTime result2;
              bool flag = DateTime.TryParse(comparedValue, out result2);
              if (num != 0)
              {
                double result3;
                if (double.TryParse(fieldValue, out result3))
                  return result3 > result1;
                if (string.IsNullOrEmpty(fieldValue))
                  return 0.0 > result1;
              }
              else
              {
                DateTime result4;
                if (flag && DateTime.TryParse(fieldValue, out result4))
                  return result4 > result2;
              }
              return string.Compare(fieldValue, comparedValue, StringComparison.Ordinal) > 0;
            }
          },
          {
            FieldConditionRuleOperator.LessThen,
             (comparedValue, fieldValue) =>
            {
              double result5;
              int num = double.TryParse(comparedValue, out result5) ? 1 : 0;
              DateTime result6;
              bool flag = DateTime.TryParse(comparedValue, out result6);
              if (num != 0)
              {
                double result7;
                if (double.TryParse(fieldValue, out result7))
                  return result7 < result5;
                if (string.IsNullOrEmpty(fieldValue))
                  return 0.0 < result5;
              }
              else
              {
                DateTime result8;
                if (flag && DateTime.TryParse(fieldValue, out result8))
                  return result8 < result6;
              }
              return string.Compare(fieldValue, comparedValue, StringComparison.Ordinal) < 0;
            }
          },
          {
            FieldConditionRuleOperator.NotContains,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.Contains(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotContainsIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.Contains(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          },
          {
            FieldConditionRuleOperator.NotStartsWith,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.StartsWith(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotStartsWithIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.StartsWith(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          },
          {
            FieldConditionRuleOperator.NotEndsWith,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.EndsWith(comparedValue);
            }
          },
          {
            FieldConditionRuleOperator.NotEndsWithIgnoreCase,
             (comparedValue, fieldValue) =>
            {
              if (fieldValue == null)
                return true;
              return comparedValue != null && !fieldValue.EndsWith(comparedValue, StringComparison.OrdinalIgnoreCase);
            }
          }
        };

                static bool Is(string comparedValue, string fieldValue) => string.Equals(comparedValue, fieldValue);
            }
        }

        public virtual bool MandatoryByDefault { get; set; }

        public virtual IEnumerable<string> RequiredJavascriptFiles(Field field) => new string[0];

        public virtual IEnumerable<string> RequiredPartialViews(Field field) => new string[0];

        public virtual IEnumerable<string> RequiredPartialViews(
          Func<string?> themeAccessor,
          Field field)
        {
            return new string[0];
        }

        public virtual IEnumerable<string> RequiredCssFiles(Field field) => new string[0];

        public virtual string RequiredJavascriptInitialization(Field field) => string.Empty;

        public Type GetDataType()
        {
            switch (this.DataType)
            {
                case FieldDataType.String:
                    return typeof(string);
                case FieldDataType.LongString:
                    return typeof(string);
                case FieldDataType.Integer:
                    return typeof(int);
                case FieldDataType.DateTime:
                    return typeof(DateTime);
                case FieldDataType.Bit:
                    return typeof(bool);
                default:
                    return typeof(string);
            }
        }

        public virtual string GetDesignView() => string.Format("~/App_Plugins/UmbracoForms/backoffice/Common/FieldTypes/{0}.html", this.GetType().Name.ToLower());

        public virtual Dictionary<string, SettingAttribute> Settings()
        {
            Dictionary<string, SettingAttribute> dictionary = new Dictionary<string, SettingAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in this.GetType().GetProperties())
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
            string input = string.Join<object>(",", postedValues.Where<object>(x => !x.ToString().IsNullOrWhiteSpace()));
            if (!string.IsNullOrEmpty(field.RegEx) && !new Regex(field.RegEx).Match(input).Success && input != string.Empty)
                FieldType.AddValidationError(field.InvalidErrorMessage, form.InvalidErrorMessage, field.Caption, placeholderParsingService, form, errors);
            if (field.Mandatory && string.IsNullOrEmpty(input) && FieldType.IsFieldVisible(form, field, fieldTypeStorage, placeholderParsingService))
                FieldType.AddValidationError(field.RequiredErrorMessage, form.RequiredErrorMessage, field.Caption, placeholderParsingService, form, errors);
            if (field.PreValues != null && field.PreValues.Any<FieldPrevalue>())
            {
                HashSet<object> source = new HashSet<object>(field.PreValues.Select<FieldPrevalue, string>(x => placeholderParsingService.ParsePlaceHolders(x.Value, false, form: form)));
                foreach (object postedValue in postedValues)
                {
                    if (!source.Contains(postedValue) && !string.IsNullOrEmpty(postedValue.ToString()))
                        errors.Add(string.Format("Unexpected value: '{0}'. Expected one of these: {1}", postedValue, string.Join(", ", source.Select<object, string>(x => string.Format("'{0}'", x)))));
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
            if (!FieldType.IsFieldVisibleWithCondition(form, field, fieldTypeStorage, placeholderParsingService))
                return false;
            FieldSet conditionedElement = form.AllFieldSets.Single<FieldSet>(x => x.Containers.SelectMany<FieldsetContainer, Field>(y => y.Fields).Any<Field>(z => z.Id == field.Id));
            return FieldType.IsFieldVisibleWithCondition(form, conditionedElement, fieldTypeStorage, placeholderParsingService);
        }

        private static bool IsFieldVisibleWithCondition(
          Form form,
          IConditioned conditionedElement,
          IFieldTypeStorage fieldTypeStorage,
          IPlaceholderParsingService placeholderParsingService)
        {
            if (!conditionedElement.HasCondition())
                return true;
            if (conditionedElement.Condition.IsCircular(form))
                return false;
            Dictionary<Guid, string> dictionary = form.AllFields.ToDictionary<Field, Guid, string>(f => f.Id, f => string.Join<object>(", ", f.Values ?? new List<object>()));
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
            return this.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage, new List<string>());
        }

        public virtual IEnumerable<object> ProcessSubmittedValue(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            object[] array = postedValues.ToArray<object>();
            return array.Length == 0 && field.Values != null && field.Values.Count > 0 ? field.Values : array;
        }

        public virtual IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = new List<object>();
            record.AddRange(postedValues);
            return record;
        }

        public virtual IEnumerable<object> ConvertFromRecord(
          Field field,
          IEnumerable<object> storedValues)
        {
            List<object> objectList = new List<object>();
            objectList.AddRange(storedValues);
            return objectList;
        }

        public virtual List<Exception> ValidateSettings() => new List<Exception>();
    }
}
