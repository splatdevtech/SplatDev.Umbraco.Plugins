
// Type: Umbraco.Forms.Web.Models.Backoffice.FormWorkflows
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "formWorkflows")]
  [Serializable]
  public class FormWorkflows
  {
    [DataMember(Name = "onSubmit")]
    public List<FormWorkflowWithTypeSettings> OnSubmit { get; set; } = new List<FormWorkflowWithTypeSettings>();

    [DataMember(Name = "onApprove")]
    public List<FormWorkflowWithTypeSettings> OnApprove { get; set; } = new List<FormWorkflowWithTypeSettings>();

    [DataMember(Name = "onReject")]
    public List<FormWorkflowWithTypeSettings> OnReject { get; set; } = new List<FormWorkflowWithTypeSettings>();
  }
}
