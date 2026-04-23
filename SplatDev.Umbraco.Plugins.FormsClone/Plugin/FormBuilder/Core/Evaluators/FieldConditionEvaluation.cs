using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

namespace FormBuilder.Core.Evaluators
{
    public static class FieldConditionEvaluation
    {
        private static readonly Dictionary<FieldConditionLogicType, Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>> s_aggregators = new()
    {
      {
        FieldConditionLogicType.All,
        new Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>(Enumerable.All)
      },
      {
        FieldConditionLogicType.Any,
        new Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>(Enumerable.Any)
      }
    };

        public static Dictionary<Guid, string> GetFormFieldValuesForConditions(
          Dictionary<string, object[]> formState,
          List<Field> allFields,
          IFieldTypeStorage fieldTypeStorage,
          HttpContext httpContext)
        {
            return (
                allFields?.Select(x => new KeyValuePair<Guid, string>(x.Id, string.Join("|", x.GetValueToStore(formState, fieldTypeStorage, httpContext, false).Select(y => ConvertToStringForConditions(x, y))))).ToDictionary(x => x.Key, x => x.Value)) ?? [];
        }

        private static string ConvertToStringForConditions(Field field, object value)
        {
            if (value is null)
                return string.Empty;
            return field.FieldTypeId == Guid.Parse("D5C0C390-AE9A-11DE-A69E-666455D89593") ? value.ToString()?.ToLowerInvariant() ?? string.Empty : value.ToString() ?? string.Empty;
        }

        private static bool Test(
          this FieldCondition condition,
          Form form,
          IFieldTypeStorage fieldTypeStorage,
          IDictionary<Guid, string> fieldValues,
          IPlaceholderParsingService placeholderParsingService)
        {
            return s_aggregators[condition.LogicType](condition.Rules, r => r.TestRule(form, fieldTypeStorage, fieldValues, placeholderParsingService));
        }

        public static bool IsCircular(this FieldCondition condition, Form form) => condition.IsCircular(form, []);

        private static bool IsCircular(
          this FieldCondition? condition,
          Form form,
          HashSet<Guid> conditionsChecked)
        {
            if (condition is null || !condition.Enabled || !condition.Rules.Any())
                return false;
            if (condition.Id == Guid.Empty)
                condition.Id = Guid.NewGuid();
            else if (conditionsChecked.Contains(condition.Id))
                return true;
            conditionsChecked.Add(condition.Id);
            Guid[] conditionFieldIds = [.. condition.Rules.Select(r => r.Field)];
            return form.AllFields.Where(x => conditionFieldIds.Contains(x.Id) && x.HasCondition()).Select(x => x.Condition).Any(x => x.IsCircular(form, conditionsChecked));
        }

        public static bool IsVisible(
          this FieldCondition? condition,
          Form form,
          IFieldTypeStorage fieldTypeStorage,
          IDictionary<Guid, string> fieldValues,
          IPlaceholderParsingService placeholderParsingService)
        {
            if (condition is null || !condition.Enabled)
                return true;
            Guid[] fieldIds = [.. condition.Rules.Select(r => r.Field)];
            FieldSet[] array1 = [.. form.Pages.SelectMany(p => p.FieldSets).Where(fs => fs.Containers.Any(c => c.Fields.Any(f => fieldIds.Contains(f.Id))))];
            Dictionary<Guid, bool> fieldsetVisibilities = array1.ToDictionary(fs => fs.Id, fs => !fs.HasCondition() || fs.Condition.IsVisible(form, fieldTypeStorage, fieldValues, placeholderParsingService));
            if (condition.LogicType == FieldConditionLogicType.All && fieldsetVisibilities.Any(v => !v.Value))
                return false;
            bool[] array2 = [.. array1.Where(fs => fieldsetVisibilities[fs.Id]).SelectMany(fs => fs.Containers).SelectMany(c => c.Fields.Where(f => fieldIds.Contains(f.Id))).Select(f => f.Condition).Select(c => c.IsVisible(form, fieldTypeStorage, fieldValues, placeholderParsingService))];
            int num = !condition.Rules.Any() ? 0 : condition.LogicType != FieldConditionLogicType.All || !array2.All(v => v) ? condition.LogicType != FieldConditionLogicType.Any ? 0 : array2.Any(v => v) ? 1 : 0 : 1;
            bool flag1 = condition.ActionType == FieldConditionActionType.Show;
            if (num == 0)
                return !flag1;
            bool flag2 = condition.Test(form, fieldTypeStorage, fieldValues, placeholderParsingService);
            if (flag2 & flag1)
                return true;
            return !flag2 && !flag1;
        }

        public static bool HasCondition(this IConditioned conditioned) => conditioned is not null && conditioned.Condition is not null && conditioned.Condition.Enabled;

        private static bool TestRule(
        this FieldConditionRule fieldCondition,
        Form form,
        IFieldTypeStorage fieldTypeStorage,
        IDictionary<Guid, string> fieldValues,
        IPlaceholderParsingService placeholderParsingService)
        {
            if (!fieldValues.TryGetValue(fieldCondition.Field, out string? fieldValue))
                return false;
            if (!TryGetFieldTypeForFieldId(form, fieldCondition.Field, fieldTypeStorage, out FieldType? fieldType))
                fieldType = new DefaultFieldType();
            Func<string, string, bool>? conditionCheckFunction = fieldType?.ConditionCheckFunctions[fieldCondition.Operator];
            string placeHolders = placeholderParsingService.ParsePlaceHolders(fieldCondition.Value, false);
            string str = fieldValue;
            return conditionCheckFunction is not null && conditionCheckFunction(placeHolders, str);
        }

        private static bool TryGetFieldTypeForFieldId(
        Form form,
        Guid fieldId,
        IFieldTypeStorage fieldTypeStorage,
        out FieldType? fieldType)
        {
            Field? field = form.AllFields.SingleOrDefault(x => x.Id == fieldId);
            if (field is null)
            {
                fieldType = null;
                return false;
            }
            fieldType = fieldTypeStorage.GetFieldTypeByField(field);
            return fieldType is not null;
        }

        private class DefaultFieldType : FieldType
        {
        }
    }
}