
// Type: Umbraco.Forms.Core.Models.FolderEntity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "folder")]
  [Serializable]
  public class FolderEntity : EntityBase
  {
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "parentKey")]
    public Guid? ParentKey { get; set; }
  }
}
