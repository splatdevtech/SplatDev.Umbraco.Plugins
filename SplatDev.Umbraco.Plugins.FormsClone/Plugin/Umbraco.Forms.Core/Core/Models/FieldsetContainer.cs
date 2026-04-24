
// Type: Umbraco.Forms.Core.Models.FieldsetContainer
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "fieldSetContainer")]
  public class FieldsetContainer : IFormObject
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "caption")]
    public string? Caption { get; set; }

    [DataMember(Name = "width")]
    public int Width { get; set; }

    [DataMember(Name = "fields")]
    public List<Field> Fields { get; set; } = new List<Field>();
  }
}
