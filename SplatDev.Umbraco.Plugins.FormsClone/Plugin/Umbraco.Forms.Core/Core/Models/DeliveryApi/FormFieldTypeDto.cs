
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormFieldTypeDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormFieldTypeDto
  {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool SupportsPreValues { get; set; }

    public bool SupportsUploadTypes { get; set; }

    public string RenderInputType { get; set; } = Umbraco.Forms.Core.Enums.RenderInputType.Single.ToString();
  }
}
