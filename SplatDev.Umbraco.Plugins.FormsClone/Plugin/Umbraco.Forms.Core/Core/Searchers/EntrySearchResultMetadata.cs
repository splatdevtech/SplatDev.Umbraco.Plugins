
// Type: Umbraco.Forms.Core.Searchers.EntrySearchResultMetadata
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  [DataContract(Name = "entrySearchResultMetadata")]
  public class EntrySearchResultMetadata
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "lastSubmittedDate")]
    public string LastSubmittedDate { get; set; } = string.Empty;
  }
}
