
// Type: Umbraco.Forms.Core.Models.FieldConditionRule
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "fieldConditionRule")]
  [Serializable]
  public class FieldConditionRule : IFormObject
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "field")]
    public Guid Field { get; set; }

    [DataMember(Name = "operator")]
    public FieldConditionRuleOperator Operator { get; set; }

    [DataMember(Name = "value")]
    public string Value { get; set; } = string.Empty;
  }
}
