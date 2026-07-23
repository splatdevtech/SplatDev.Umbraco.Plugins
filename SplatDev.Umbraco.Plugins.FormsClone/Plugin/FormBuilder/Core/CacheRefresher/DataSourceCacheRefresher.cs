using FormBuilder.Core.DataSources;
using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Serialization;

using static FormBuilder.Constants;

namespace FormBuilder.Core.CacheRefresher
{
    internal sealed class DataSourceCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory) :
        PayloadCacheRefresherBase<DataSourceCacheRefresherNotification, JsonPayloads.DataSourcePayload>(appCache, serializer, eventAggregator, factory)
    {
        public override Guid RefresherUniqueId => FormBuilderCacheKeys.DataSourceDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms DataSource Cache Refresher";

        public override void RefreshAll()
        {
            AppCaches.RuntimeCache.ClearByKey("Forms.DataSource.");
            base.RefreshAll();
        }

        public override void Refresh(JsonPayloads.DataSourcePayload[] payload)
        {
            foreach (FormDataSource? formDataSource in payload.Select(x => x.DataSource))
            {
                if (formDataSource is not null)
                    AppCaches.RuntimeCache.ClearByKey("Forms.DataSource." + formDataSource.Id.ToString());
            }
            base.Refresh(payload);
        }
    }
}