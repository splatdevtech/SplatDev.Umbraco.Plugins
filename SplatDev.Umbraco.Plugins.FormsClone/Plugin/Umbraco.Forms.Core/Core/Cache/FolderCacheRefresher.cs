
// Type: Umbraco.Forms.Core.Cache.FolderCacheRefresher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
    internal sealed class FolderCacheRefresher :
      PayloadCacheRefresherBase<FolderCacheRefresherNotification, JsonPayloads.FolderPayload>
    {
        public FolderCacheRefresher(
          AppCaches appCache,
          IJsonSerializer serializer,
          IEventAggregator eventAggregator,
          ICacheRefresherNotificationFactory factory)
          : base(appCache, serializer, eventAggregator, factory)
        {
        }

        public override Guid RefresherUniqueId => CacheKeys.FoldersDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms Folders Cache Refresher";

        public override void Refresh(JsonPayloads.FolderPayload[] payload)
        {
            this.AppCaches.RuntimeCache.ClearByKey("Forms.Folder.");
            base.Refresh(payload);
        }
    }
}
