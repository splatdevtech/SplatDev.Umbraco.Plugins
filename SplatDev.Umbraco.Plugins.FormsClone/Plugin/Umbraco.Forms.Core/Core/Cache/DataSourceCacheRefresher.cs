
// Type: Umbraco.Forms.Core.Cache.DataSourceCacheRefresher
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
    internal sealed class DataSourceCacheRefresher :
      PayloadCacheRefresherBase<DataSourceCacheRefresherNotification, JsonPayloads.DataSourcePayload>
    {
        public DataSourceCacheRefresher(
          AppCaches appCache,
          IJsonSerializer serializer,
          IEventAggregator eventAggregator,
          ICacheRefresherNotificationFactory factory)
          : base(appCache, serializer, eventAggregator, factory)
        {
        }

        public override Guid RefresherUniqueId => CacheKeys.DataSourceDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms DataSource Cache Refresher";

        public override void RefreshAll()
        {
            this.AppCaches.RuntimeCache.ClearByKey("Forms.DataSource.");
            base.RefreshAll();
        }

        public override void Refresh(JsonPayloads.DataSourcePayload[] payload)
        {
            foreach (FormDataSource formDataSource in payload.Select<JsonPayloads.DataSourcePayload, FormDataSource>(x => x.DataSource))
            {
                if (formDataSource != null)
                    this.AppCaches.RuntimeCache.ClearByKey("Forms.DataSource." + formDataSource.Id.ToString());
            }
            base.Refresh(payload);
        }
    }
}
