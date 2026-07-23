
// Type: Umbraco.Forms.Web.Models.FieldsetViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class FieldsetViewModel
  {
    public string Id { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public bool HasCondition { get; set; }

    public FieldConditionActionType ConditionActionType { get; set; }

    public FieldConditionLogicType ConditionLogicType { get; set; }

    public IEnumerable<FieldConditionRule> ConditionRules { get; set; } = Enumerable.Empty<FieldConditionRule>();

    public IEnumerable<FieldCondition> ParentConditions { get; set; } = Enumerable.Empty<FieldCondition>();

    public FieldCondition? Condition { get; set; }

    public IList<FieldsetContainerViewModel> Containers { get; set; } = (IList<FieldsetContainerViewModel>) new List<FieldsetContainerViewModel>();
  }
}
