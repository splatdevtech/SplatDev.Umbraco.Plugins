
// Type: Umbraco.Forms.Web.Models.ConditionViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class ConditionViewModel
  {
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("actionType")]
    public FieldConditionActionType ActionType { get; set; }

    [JsonPropertyName("logicType")]
    public FieldConditionLogicType LogicType { get; set; }

    [JsonPropertyName("rules")]
    public IEnumerable<ConditionRuleViewModel> Rules { get; set; } = Enumerable.Empty<ConditionRuleViewModel>();

    public static ConditionViewModel Build(
      Form form,
      FieldCondition condition,
      IPlaceholderParsingService placeholderParsingService)
    {
      return new ConditionViewModel()
      {
        Id = condition.Id,
        ActionType = condition.ActionType,
        LogicType = condition.LogicType,
        Rules = condition.Rules.Select<FieldConditionRule, ConditionRuleViewModel>((Func<FieldConditionRule, ConditionRuleViewModel>) (rule => ConditionRuleViewModel.Build(form, rule, placeholderParsingService)))
      };
    }
  }
}
