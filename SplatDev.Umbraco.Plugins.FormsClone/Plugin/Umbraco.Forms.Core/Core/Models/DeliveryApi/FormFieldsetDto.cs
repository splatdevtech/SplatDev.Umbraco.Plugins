
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormFieldsetDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormFieldsetDto
  {
    public Guid Id { get; set; }

    public string? Caption { get; set; }

    public FormConditionDto? Condition { get; set; }

    public IEnumerable<FormFieldsetColumnDto> Columns { get; set; } = (IEnumerable<FormFieldsetColumnDto>) new List<FormFieldsetColumnDto>();
  }
}
