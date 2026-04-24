
// Type: Umbraco.Forms.Core.Searchers.EntrySearchResultFetchingNotification
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.Searchers
{
  public sealed class EntrySearchResultFetchingNotification : ObjectNotification<EntrySearchResult>
  {
    public EntrySearchResultFetchingNotification(EntrySearchResult target, EventMessages messages)
      : base(target, messages)
    {
    }

    public EntrySearchResult EntrySearchResult => this.Target;
  }
}
