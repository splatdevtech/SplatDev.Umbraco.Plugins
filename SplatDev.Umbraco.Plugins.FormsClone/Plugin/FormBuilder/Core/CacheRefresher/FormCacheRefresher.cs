using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;

using static FormBuilder.Constants;

namespace FormBuilder.Core.CacheRefresher
{
    internal sealed class FormCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory) :
        PayloadCacheRefresherBase<FormCacheRefresherNotification, JsonPayloads.FormPayload>(appCache, serializer, eventAggregator, factory)
    {
        public override Guid RefresherUniqueId => FormBuilderCacheKeys.FormsDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms Cache Refresher";

        public override void Refresh(JsonPayloads.FormPayload[] payload)
        {
            AppCaches.RuntimeCache.Clear("Forms.FormStorage.All");
            base.Refresh(payload);
        }
    }
}