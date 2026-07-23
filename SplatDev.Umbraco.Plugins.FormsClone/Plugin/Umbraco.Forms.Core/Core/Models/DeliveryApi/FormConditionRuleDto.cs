
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormConditionRuleDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormConditionRuleDto
  {
    public string Field { get; set; } = string.Empty;

    public FieldConditionRuleOperator Operator { get; set; }

    public string Value { get; set; } = string.Empty;
  }
}
