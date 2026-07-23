
// Type: Umbraco.Forms.Web.Models.Backoffice.FormWorkflowWithTypeSettings
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "workflow")]
  [Serializable]
  public class FormWorkflowWithTypeSettings
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "form")]
    public Guid Form { get; set; }

    [DataMember(Name = "active")]
    public bool Active { get; set; }

    [DataMember(Name = "includeSensitiveData")]
    public IncludeSensitiveData IncludeSensitiveData { get; set; }

    [DataMember(Name = "isDeleted")]
    public bool IsDeleted { get; set; }

    [DataMember(Name = "sortOrder")]
    public int SortOrder { get; set; }

    [DataMember(Name = "workflowTypeId")]
    public Guid WorkflowTypeId { get; set; }

    [DataMember(Name = "workflowTypeName")]
    public string WorkflowTypeName { get; set; } = string.Empty;

    [DataMember(Name = "workflowTypeDescription")]
    public string WorkflowTypeDescription { get; set; } = string.Empty;

    [DataMember(Name = "workflowTypeIcon")]
    public string WorkflowTypeIcon { get; set; } = string.Empty;

    [DataMember(Name = "workflowTypeGroup")]
    public string WorkflowTypeGroup { get; set; } = string.Empty;

    [DataMember(Name = "settings")]
    public IDictionary<string, string> Settings { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [DataMember(Name = "isMandatory")]
    public bool IsMandatory { get; set; }

    [DataMember(Name = "condition")]
    public FieldCondition? Condition { get; set; }
  }
}
