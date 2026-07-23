
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormValidationRuleDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormValidationRuleDto
  {
    public string Rule { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public Guid FieldId { get; set; }
  }
}
