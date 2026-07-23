using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;

namespace FormBuilder.Core.Notifications
{
    public class PreValueCacheRefresherNotification(object messageObject, MessageType messageType) : CacheRefresherNotification(messageObject, messageType)
    {
    }
}