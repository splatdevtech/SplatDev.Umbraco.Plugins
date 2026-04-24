
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormFieldsetColumnDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormFieldsetColumnDto
  {
    public string? Caption { get; set; }

    public int Width { get; set; }

    public IEnumerable<FormFieldDto> Fields { get; set; } = (IEnumerable<FormFieldDto>) new List<FormFieldDto>();
  }
}
