
// Type: Umbraco.Forms.Web.Models.Backoffice.FormSecurityForGroup
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  [DataContract(Name = "formSecurityForGroup")]
  [Serializable]
  public class FormSecurityForGroup
  {
    [DataMember(Name = "key")]
    public Guid Key { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "unique")]
    public Guid Unique => this.Key;

    [DataMember(Name = "entityType")]
    public string EntityType => "forms-security-user-group";

    [DataMember(Name = "userGroupSecurity")]
    public UserGroupSecurity UserGroupSecurity { get; set; } = new UserGroupSecurity();

    [DataMember(Name = "startFolders")]
    public List<Guid> StartFolderIds { get; set; } = new List<Guid>();

    [DataMember(Name = "formsSecurity")]
    public List<UserGroupFormSecurity> FormsSecurity { get; set; } = new List<UserGroupFormSecurity>();
  }
}
