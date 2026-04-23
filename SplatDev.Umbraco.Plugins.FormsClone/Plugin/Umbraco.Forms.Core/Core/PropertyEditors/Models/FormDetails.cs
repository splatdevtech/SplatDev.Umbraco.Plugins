
// Type: Umbraco.Forms.Core.PropertyEditors.Models.FormDetails
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors.Models
{
  public class FormDetails
  {
    public Guid? FormId { get; set; }

    public string? Theme { get; set; }

    public Guid? RedirectToPageId { get; set; }
  }
}
