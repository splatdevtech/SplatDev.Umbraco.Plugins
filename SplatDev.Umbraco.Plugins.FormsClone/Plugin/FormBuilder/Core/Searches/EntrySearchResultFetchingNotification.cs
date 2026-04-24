using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Searches
{
    public sealed class EntrySearchResultFetchingNotification(EntrySearchResult target, EventMessages messages) : ObjectNotification<EntrySearchResult>(target, messages)
    {
        public EntrySearchResult EntrySearchResult => Target;
    }
}