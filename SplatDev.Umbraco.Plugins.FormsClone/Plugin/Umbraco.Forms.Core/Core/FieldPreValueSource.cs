
// Type: Umbraco.Forms.Core.FieldPreValueSource
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core
{
  [DataContract(Name = "fieldPreValueSource")]
  public class FieldPreValueSource : ITypeWithEditorDetails, IType
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "unique")]
    public Guid Unique => this.Id;

    [DataMember(Name = "entityType")]
    public string EntityType => "prevaluesource";

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "created")]
    public DateTime Created { get; set; }

    [DataMember(Name = "createdBy")]
    public int? CreatedBy { get; set; }

    [DataMember(Name = "createdByName")]
    public string? CreatedByName { get; set; }

    [DataMember(Name = "updated")]
    public DateTime Updated { get; set; }

    [DataMember(Name = "updatedBy")]
    public int? UpdatedBy { get; set; }

    [DataMember(Name = "updatedByName")]
    public string? UpdatedByName { get; set; }

    [DataMember(Name = "settings")]
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [DataMember(Name = "fieldPreValueSourceTypeId")]
    public Guid FieldPreValueSourceTypeId { get; set; }

    [DataMember(Name = "cachePrevaluesFor")]
    public TimeSpan CachePrevaluesFor { get; set; } = TimeSpan.FromMilliseconds(-1L, 0L);
  }
}
