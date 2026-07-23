using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications;
using FormBuilder.Core.Prevalues;

using Umbraco.Cms.Core.Cache;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Serialization;

using static FormBuilder.Constants;

namespace FormBuilder.Core.CacheRefresher
{
    internal sealed class PreValueCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory) :
        PayloadCacheRefresherBase<PreValueCacheRefresherNotification, JsonPayloads.PreValuePayload>(appCache, serializer, eventAggregator, factory)
    {
        public override Guid RefresherUniqueId => FormBuilderCacheKeys.PreValueDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms Prevalue Cache Refresher";

        public override void RefreshAll()
        {
            AppCaches.RuntimeCache.ClearByKey("Forms.PreValues.");
            base.RefreshAll();
        }

        public override void Refresh(JsonPayloads.PreValuePayload[] payload)
        {
            foreach (FieldPrevalueSource? fieldPreValueSource in payload.Select(x => x.Prevalue))
            {
                if (fieldPreValueSource is not null)
                    AppCaches.RuntimeCache.ClearByKey("Forms.PreValues." + fieldPreValueSource.Id.ToString());
            }
            base.Refresh(payload);
        }
    }
}