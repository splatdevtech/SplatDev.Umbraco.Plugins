
// Type: Umbraco.Forms.Web.Models.Backoffice.RecordWorkflowAuditEntry
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "recordWorkflowAuditEntry")]
  [Serializable]
  public class RecordWorkflowAuditEntry
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "workflowKey")]
    public Guid WorkflowKey { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "typeName")]
    public string TypeName { get; set; } = string.Empty;

    [DataMember(Name = "executedOn")]
    public DateTime ExecutedOn { get; set; }

    [DataMember(Name = "executionStage")]
    public string ExecutionStage { get; set; } = string.Empty;

    [DataMember(Name = "result")]
    public string Result { get; set; } = string.Empty;
  }
}
