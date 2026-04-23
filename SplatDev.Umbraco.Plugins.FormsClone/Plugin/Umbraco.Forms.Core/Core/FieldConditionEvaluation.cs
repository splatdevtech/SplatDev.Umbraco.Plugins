
// Type: Umbraco.Forms.Core.FieldConditionEvaluation
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core
{
  public static class FieldConditionEvaluation
  {
    private static readonly Dictionary<FieldConditionLogicType, Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>> s_aggregators = new Dictionary<FieldConditionLogicType, Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>>()
    {
      {
        FieldConditionLogicType.All,
        new Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>(Enumerable.All<FieldConditionRule>)
      },
      {
        FieldConditionLogicType.Any,
        new Func<IEnumerable<FieldConditionRule>, Func<FieldConditionRule, bool>, bool>(Enumerable.Any<FieldConditionRule>)
      }
    };

    public static Dictionary<Guid, string> GetFormFieldValuesForConditions(
      Dictionary<string, object[]> formState,
      List<Field> allFields,
      IFieldTypeStorage fieldTypeStorage,
      HttpContext httpContext)
    {
      return (allFields != null ? allFields.Select<Field, KeyValuePair<Guid, string>>((Func<Field, KeyValuePair<Guid, string>>) (x => new KeyValuePair<Guid, string>(x.Id, string.Join("|", ((IEnumerable<object>) x.GetValueToStore(formState, fieldTypeStorage, httpContext, false)).Select<object, string>((Func<object, string>) (y => FieldConditionEvaluation.ConvertToStringForConditions(x, y))))))).ToDictionary<KeyValuePair<Guid, string>, Guid, string>((Func<KeyValuePair<Guid, string>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, string>, string>) (x => x.Value)) : (Dictionary<Guid, string>) null) ?? new Dictionary<Guid, string>();
    }

    private static string ConvertToStringForConditions(Field field, object value)
    {
      if (value == null)
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
      return FieldConditionEvaluation.s_aggregators[condition.LogicType](condition.Rules, (Func<FieldConditionRule, bool>) (r => r.TestRule(form, fieldTypeStorage, fieldValues, placeholderParsingService)));
    }

    public static bool IsCircular(this FieldCondition condition, Form form) => condition.IsCircular(form, new HashSet<Guid>());

    private static bool IsCircular(
      this FieldCondition condition,
      Form form,
      HashSet<Guid> conditionsChecked)
    {
      if (condition == null || !condition.Enabled || !condition.Rules.Any<FieldConditionRule>())
        return false;
      if (condition.Id == Guid.Empty)
        condition.Id = Guid.NewGuid();
      else if (conditionsChecked.Contains(condition.Id))
        return true;
      conditionsChecked.Add(condition.Id);
      Guid[] conditionFieldIds = condition.Rules.Select<FieldConditionRule, Guid>((Func<FieldConditionRule, Guid>) (r => r.Field)).ToArray<Guid>();
      return form.AllFields.Where<Field>((Func<Field, bool>) (x => ((IEnumerable<Guid>) conditionFieldIds).Contains<Guid>(x.Id) && x.HasCondition())).Select<Field, FieldCondition>((Func<Field, FieldCondition>) (x => x.Condition)).Any<FieldCondition>((Func<FieldCondition, bool>) (x => x.IsCircular(form, conditionsChecked)));
    }

    public static bool IsVisible(
      this FieldCondition condition,
      Form form,
      IFieldTypeStorage fieldTypeStorage,
      IDictionary<Guid, string> fieldValues,
      IPlaceholderParsingService placeholderParsingService)
    {
      if (condition == null || !condition.Enabled)
        return true;
      Guid[] fieldIds = condition.Rules.Select<FieldConditionRule, Guid>((Func<FieldConditionRule, Guid>) (r => r.Field)).ToArray<Guid>();
      FieldSet[] array1 = form.Pages.SelectMany<Page, FieldSet>((Func<Page, IEnumerable<FieldSet>>) (p => (IEnumerable<FieldSet>) p.FieldSets)).Where<FieldSet>((Func<FieldSet, bool>) (fs => fs.Containers.Any<FieldsetContainer>((Func<FieldsetContainer, bool>) (c => c.Fields.Any<Field>((Func<Field, bool>) (f => ((IEnumerable<Guid>) fieldIds).Contains<Guid>(f.Id))))))).ToArray<FieldSet>();
      Dictionary<Guid, bool> fieldsetVisibilities = ((IEnumerable<FieldSet>) array1).ToDictionary<FieldSet, Guid, bool>((Func<FieldSet, Guid>) (fs => fs.Id), (Func<FieldSet, bool>) (fs => !fs.HasCondition() || fs.Condition.IsVisible(form, fieldTypeStorage, fieldValues, placeholderParsingService)));
      if (condition.LogicType == FieldConditionLogicType.All && fieldsetVisibilities.Any<KeyValuePair<Guid, bool>>((Func<KeyValuePair<Guid, bool>, bool>) (v => !v.Value)))
        return false;
      bool[] array2 = ((IEnumerable<FieldSet>) array1).Where<FieldSet>((Func<FieldSet, bool>) (fs => fieldsetVisibilities[fs.Id])).SelectMany<FieldSet, FieldsetContainer>((Func<FieldSet, IEnumerable<FieldsetContainer>>) (fs => (IEnumerable<FieldsetContainer>) fs.Containers)).SelectMany<FieldsetContainer, Field>((Func<FieldsetContainer, IEnumerable<Field>>) (c => c.Fields.Where<Field>((Func<Field, bool>) (f => ((IEnumerable<Guid>) fieldIds).Contains<Guid>(f.Id))))).Select<Field, FieldCondition>((Func<Field, FieldCondition>) (f => f.Condition)).Select<FieldCondition, bool>((Func<FieldCondition, bool>) (c => c.IsVisible(form, fieldTypeStorage, fieldValues, placeholderParsingService))).ToArray<bool>();
      int num = !condition.Rules.Any<FieldConditionRule>() ? 0 : (condition.LogicType != FieldConditionLogicType.All || !((IEnumerable<bool>) array2).All<bool>((Func<bool, bool>) (v => v)) ? (condition.LogicType != FieldConditionLogicType.Any ? 0 : (((IEnumerable<bool>) array2).Any<bool>((Func<bool, bool>) (v => v)) ? 1 : 0)) : 1);
      bool flag1 = condition.ActionType == FieldConditionActionType.Show;
      if (num == 0)
        return !flag1;
      bool flag2 = condition.Test(form, fieldTypeStorage, fieldValues, placeholderParsingService);
      if (flag2 & flag1)
        return true;
      return !flag2 && !flag1;
    }

    public static bool HasCondition(this IConditioned conditioned) => conditioned != null && conditioned.Condition != null && conditioned.Condition.Enabled;

    private static bool TestRule(
      this FieldConditionRule fieldCondition,
      Form form,
      IFieldTypeStorage fieldTypeStorage,
      IDictionary<Guid, string> fieldValues,
      IPlaceholderParsingService placeholderParsingService)
    {
      if (!fieldValues.ContainsKey(fieldCondition.Field))
        return false;
      FieldType fieldType;
      if (!FieldConditionEvaluation.TryGetFieldTypeForFieldId(form, fieldCondition.Field, fieldTypeStorage, out fieldType))
        fieldType = (FieldType) new FieldConditionEvaluation.DefaultFieldType();
      Func<string, string, bool> conditionCheckFunction = fieldType.ConditionCheckFunctions[fieldCondition.Operator];
      string fieldValue = fieldValues[fieldCondition.Field];
      string placeHolders = placeholderParsingService.ParsePlaceHolders(fieldCondition.Value, false);
      string str = fieldValue;
      return conditionCheckFunction(placeHolders, str);
    }

    private static bool TryGetFieldTypeForFieldId(
      Form form,
      Guid fieldId,
      IFieldTypeStorage fieldTypeStorage,
      out FieldType? fieldType)
    {
      Field field = form.AllFields.SingleOrDefault<Field>((Func<Field, bool>) (x => x.Id == fieldId));
      if (field == null)
      {
        fieldType = (FieldType) null;
        return false;
      }
      fieldType = fieldTypeStorage.GetFieldTypeByField(field);
      return fieldType != null;
    }

    private class DefaultFieldType : FieldType
    {
    }
  }
}
