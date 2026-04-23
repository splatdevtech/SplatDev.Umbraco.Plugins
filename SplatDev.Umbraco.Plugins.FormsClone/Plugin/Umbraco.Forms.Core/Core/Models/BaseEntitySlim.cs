
// Type: Umbraco.Forms.Core.Models.BaseEntitySlim
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  public abstract class BaseEntitySlim
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "key")]
    public Guid Key { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "createDate")]
    public DateTime CreateDate { get; set; }
  }
}
