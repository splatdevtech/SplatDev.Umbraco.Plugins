using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;

using static FormBuilder.Constants;

namespace FormBuilder.Core.CacheRefresher
{
    internal sealed class FolderCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory) :
        PayloadCacheRefresherBase<FolderCacheRefresherNotification, JsonPayloads.FolderPayload>(appCache, serializer, eventAggregator, factory)
    {
        public override Guid RefresherUniqueId => FormBuilderCacheKeys.FoldersDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms Folders Cache Refresher";

        public override void Refresh(JsonPayloads.FolderPayload[] payload)
        {
            AppCaches.RuntimeCache.ClearByKey("Forms.Folder.");
            base.Refresh(payload);
        }
    }
}