
// Type: Umbraco.Forms.Core.Searchers.EntrySearchResult
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  [DataContract(Name = "entrySearchResult")]
  [Serializable]
  public class EntrySearchResult
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "score")]
    public float Score { get; set; }

    [DataMember(Name = "form")]
    public string Form { get; set; } = string.Empty;

    [DataMember(Name = "state")]
    public string State { get; set; } = string.Empty;

    [DataMember(Name = "created")]
    public DateTime Created { get; set; }

    [DataMember(Name = "updated")]
    public DateTime Updated { get; set; }

    [DataMember(Name = "uniqueId")]
    public Guid UniqueId { get; set; }

    [DataMember(Name = "fields")]
    public IEnumerable<EntrySearchResult.FieldData> Fields { get; set; } = (IEnumerable<EntrySearchResult.FieldData>) new List<EntrySearchResult.FieldData>();

    [DataMember(Name = "member")]
    public EntrySearchResult.MemberData? Member { get; set; }

    [DataMember(Name = "umbracoPage")]
    public UmbracoPageDetail? UmbracoPage { get; set; }

    [DataMember(Name = "culture")]
    public string Culture { get; set; } = string.Empty;

    [DataMember(Name = "numberOfWorkflowsExecuted")]
    public int NumberOfWorkflowsExecuted { get; set; }

    [DataMember(Name = "numberOfWorkflowsCompleted")]
    public int NumberOfWorkflowsCompleted { get; set; }

    [DataContract(Name = "entrySearchResultFieldData")]
    [Serializable]
    public class FieldData
    {
      [DataMember(Name = "fieldId")]
      public string FieldId { get; set; } = string.Empty;

      [DataMember(Name = "value")]
      public object? Value { get; set; }
    }

    [DataContract(Name = "entrySearchResultMemberData")]
    [Serializable]
    public class MemberData
    {
      [DataMember(Name = "name")]
      public string Name { get; set; } = string.Empty;

      [DataMember(Name = "email")]
      public string Email { get; set; } = string.Empty;

      [DataMember(Name = "unique")]
      public Guid Unique { get; set; }
    }
  }
}
