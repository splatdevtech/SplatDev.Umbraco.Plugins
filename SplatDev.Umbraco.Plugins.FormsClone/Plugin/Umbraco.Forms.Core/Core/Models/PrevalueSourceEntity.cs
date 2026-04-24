
// Type: Umbraco.Forms.Core.Models.PrevalueSourceEntity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models.Entities;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "preValue", Namespace = "")]
  [Serializable]
  public class PrevalueSourceEntity : EntityBase
  {
    [DataMember(Name = "value")]
    [JsonPropertyName("value")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "createdBy")]
    public int? CreatedBy { get; set; }

    [DataMember(Name = "updatedBy")]
    public int? UpdatedBy { get; set; }

    [DataMember(Name = "settings")]
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [DataMember(Name = "fieldPreValueSourceTypeId")]
    public Guid FieldPreValueSourceTypeId { get; set; }

    [DataMember(Name = "cachePrevaluesFor")]
    public TimeSpan CachePrevaluesFor { get; set; } = TimeSpan.FromMilliseconds(-1L, 0L);
  }
}
