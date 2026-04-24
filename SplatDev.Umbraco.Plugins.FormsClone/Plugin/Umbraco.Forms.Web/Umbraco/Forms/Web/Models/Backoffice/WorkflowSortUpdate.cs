
// Type: Umbraco.Forms.Web.Models.Backoffice.WorkflowSortUpdate
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "workflowSortUpdate")]
  [Serializable]
  public class WorkflowSortUpdate
  {
    [DataMember(Name = "state")]
    public string State { get; set; } = string.Empty;

    [DataMember(Name = "guids")]
    public string[] Guids { get; set; } = Array.Empty<string>();
  }
}
