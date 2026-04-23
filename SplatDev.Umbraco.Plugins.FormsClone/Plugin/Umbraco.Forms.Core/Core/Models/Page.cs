
// Type: Umbraco.Forms.Core.Models.Page
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "page")]
  [Serializable]
  public class Page : IFormObject
  {
    [DataMember(Name = "fieldSets")]
    public List<FieldSet> FieldSets { get; set; } = new List<FieldSet>();

    [DataMember(Name = "caption")]
    public string? Caption { get; set; }

    [DataMember(Name = "sortOrder")]
    public int SortOrder { get; set; }

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "form")]
    public Guid Form { get; set; }

    [DataMember(Name = "buttonCondition")]
    public FieldCondition? ButtonCondition { get; set; }

    public IEnumerable<Field> AllFields() => this.FieldSets.SelectMany<FieldSet, FieldsetContainer>((Func<FieldSet, IEnumerable<FieldsetContainer>>) (x => (IEnumerable<FieldsetContainer>) x.Containers)).SelectMany<FieldsetContainer, Field>((Func<FieldsetContainer, IEnumerable<Field>>) (x => (IEnumerable<Field>) x.Fields));
  }
}
