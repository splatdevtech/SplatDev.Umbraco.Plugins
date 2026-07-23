
// Type: Umbraco.Forms.Web.Models.Backoffice.WorkflowTypeWithSettings
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "workflowType")]
  [Serializable]
  public class WorkflowTypeWithSettings : IProviderTypeWithSettings
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "unique")]
    public Guid Unique => this.Id;

    [DataMember(Name = "entityType")]
    public string EntityType => "workflow-type";

    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "description")]
    public string Description { get; set; } = string.Empty;

    [DataMember(Name = "icon")]
    public string Icon { get; set; } = string.Empty;

    [DataMember(Name = "group")]
    public string Group { get; set; } = string.Empty;

    [DataMember(Name = "settings")]
    public IEnumerable<Setting> Settings { get; set; } = Enumerable.Empty<Setting>();
  }
}
