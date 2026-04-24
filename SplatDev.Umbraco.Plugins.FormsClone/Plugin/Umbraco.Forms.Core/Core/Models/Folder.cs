
// Type: Umbraco.Forms.Core.Models.Folder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "folder", Namespace = "")]
  [Serializable]
  public class Folder : IType
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [Required]
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "created")]
    public DateTime Created { get; set; }

    [DataMember(Name = "parentId")]
    public Guid? ParentId { get; set; }
  }
}
