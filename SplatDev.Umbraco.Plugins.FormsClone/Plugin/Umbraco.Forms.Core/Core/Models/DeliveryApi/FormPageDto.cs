
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormPageDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormPageDto
  {
    public string? Caption { get; set; }

    public FormConditionDto? Condition { get; set; }

    public IEnumerable<FormFieldsetDto> Fieldsets { get; set; } = (IEnumerable<FormFieldsetDto>) new List<FormFieldsetDto>();
  }
}
