
// Type: Umbraco.Forms.Core.Models.FieldSet
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "fieldSet")]
  [Serializable]
  public class FieldSet : IFormObject, IConditioned
  {
    [DataMember(Name = "caption")]
    public string? Caption { get; set; }

    [DataMember(Name = "sortOrder")]
    public int SortOrder { get; set; }

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "page")]
    public Guid Page { get; set; }

    [DataMember(Name = "containers")]
    public List<FieldsetContainer> Containers { get; set; } = new List<FieldsetContainer>();

    [DataMember(Name = "condition")]
    public FieldCondition? Condition { get; set; }

    [XmlIgnore]
    [IgnoreDataMember]
    [JsonIgnore]
    public List<Field> AllFields
    {
      get
      {
        List<Field> allFields = new List<Field>();
        foreach (FieldsetContainer container in this.Containers)
        {
          foreach (Field field in container.Fields)
            allFields.Add(field);
        }
        return allFields;
      }
    }
  }
}
