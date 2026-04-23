
// Type: Umbraco.Forms.Web.Models.ManagementApi.Picker.MappedDocumentTypeModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Web.Models.ManagementApi.Picker
{
  public class MappedDocumentTypeModel
  {
    public string DoctypeAlias { get; set; } = string.Empty;

    public List<MappedDocumentTypePropertyModel> CurrentProperties { get; set; } = new List<MappedDocumentTypePropertyModel>();
  }
}
