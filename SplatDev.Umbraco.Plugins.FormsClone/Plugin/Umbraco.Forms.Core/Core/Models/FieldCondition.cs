
// Type: Umbraco.Forms.Core.Models.FieldCondition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "fieldCondition")]
  [Serializable]
  public class FieldCondition : IFormObject
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "enabled")]
    public bool Enabled { get; set; }

    [DataMember(Name = "actionType")]
    public FieldConditionActionType ActionType { get; set; }

    [DataMember(Name = "logicType")]
    public FieldConditionLogicType LogicType { get; set; }

    [DataMember(Name = "rules")]
    public IEnumerable<FieldConditionRule> Rules { get; set; } = (IEnumerable<FieldConditionRule>) new List<FieldConditionRule>();
  }
}
