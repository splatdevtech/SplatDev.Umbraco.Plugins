
// Type: Umbraco.Forms.Core.Searchers.EntrySearchResultSchema
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  [DataContract(Name = "entrySearchResultSchema")]
  public class EntrySearchResultSchema
  {
    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "alias")]
    public string Alias { get; set; } = string.Empty;

    [DataMember(Name = "view")]
    public string View { get; set; } = string.Empty;

    [DataMember(Name = "editView")]
    public string EditView { get; set; } = string.Empty;

    [DataMember(Name = "id")]
    public string Id { get; set; } = string.Empty;

    [DataMember(Name = "containsSensitiveData")]
    public bool ContainsSensitiveData { get; set; }

    [DataMember(Name = "showOnListingScreen")]
    public bool ShowOnListingScreen { get; set; } = true;
  }
}
