
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormConditionDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormConditionDto
  {
    public FieldConditionActionType ActionType { get; set; }

    public FieldConditionLogicType LogicType { get; set; }

    public IEnumerable<FormConditionRuleDto> Rules { get; set; } = (IEnumerable<FormConditionRuleDto>) new List<FormConditionRuleDto>();
  }
}
