
// Type: Umbraco.Forms.Web.Models.Backoffice.RecordAuditEntry
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "recordAuditEntry")]
  [Serializable]
  public class RecordAuditEntry
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "updatedOn")]
    public DateTime UpdatedOn { get; set; }

    [DataMember(Name = "updatedBy")]
    public string UpdatedBy { get; set; } = string.Empty;
  }
}
