
// Type: Umbraco.Forms.Core.Models.RecordExportFilter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "entryExportFilterModel")]
  public class RecordExportFilter : RecordFilter
  {
    [DataMember(Name = "exportType")]
    public string ExportType { get; set; } = string.Empty;
  }
}
