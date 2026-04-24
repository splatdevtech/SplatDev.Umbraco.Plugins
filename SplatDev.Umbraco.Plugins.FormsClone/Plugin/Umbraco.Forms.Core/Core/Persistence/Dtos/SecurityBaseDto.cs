
// Type: Umbraco.Forms.Core.Persistence.Dtos.SecurityBaseDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [DataContract(Name = "securityBase")]
  public abstract class SecurityBaseDto
  {
    [DataMember(Name = "manageDataSources")]
    public bool ManageDataSources { get; set; }

    [DataMember(Name = "managePreValueSources")]
    public bool ManagePreValueSources { get; set; }

    [DataMember(Name = "manageWorkflows")]
    public bool ManageWorkflows { get; set; }

    [DataMember(Name = "manageForms")]
    public bool ManageForms { get; set; }

    [DataMember(Name = "viewEntries")]
    [Constraint(Default = "0")]
    public bool ViewEntries { get; set; }

    [DataMember(Name = "editEntries")]
    [Constraint(Default = "0")]
    public bool EditEntries { get; set; }

    [DataMember(Name = "deleteEntries")]
    [Constraint(Default = "0")]
    public bool DeleteEntries { get; set; }
  }
}
