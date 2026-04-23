
// Type: Umbraco.Forms.Core.Models.WorkflowEntity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "workflow", Namespace = "")]
  [Serializable]
  public class WorkflowEntity : EntityBase
  {
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "form")]
    [JsonPropertyName("form")]
    public Guid FormId { get; set; }

    [DataMember(Name = "active")]
    public bool Active { get; set; }

    [JsonConverter(typeof (JsonSensitiveDataConverter))]
    [DataMember(Name = "includeSensitiveData")]
    public IncludeSensitiveData IncludeSensitiveData { get; set; } = IncludeSensitiveData.Undefined;

    [DataMember(Name = "workflowTypeId")]
    public Guid WorkflowTypeId { get; set; }

    [DataMember(Name = "executesOn")]
    public FormState ExecutesOn { get; set; }

    [DataMember(Name = "settings")]
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [DataMember(Name = "sortOrder")]
    public int SortOrder { get; set; }

    [DataMember(Name = "isMandatory")]
    public bool IsMandatory { get; set; }

    [DataMember(Name = "condition")]
    public FieldCondition? Condition { get; set; }
  }
}
