
// Type: Umbraco.Forms.Core.Searchers.EntrySearchResultCollection
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  [DataContract(Name = "entrySearchResultCollection")]
  public class EntrySearchResultCollection
  {
    [DataMember(Name = "totalNumberOfResults")]
    public long TotalNumberOfResults { get; set; }

    [DataMember(Name = "totalNumberOfPages")]
    public long TotalNumberOfPages { get; set; }

    [DataMember(Name = "schema")]
    public IEnumerable<EntrySearchResultSchema> Schema { get; set; } = (IEnumerable<EntrySearchResultSchema>) new List<EntrySearchResultSchema>();

    [DataMember(Name = "results")]
    public IEnumerable<EntrySearchResult> Results { get; set; } = (IEnumerable<EntrySearchResult>) new List<EntrySearchResult>();
  }
}
