using FormBuilder.Core.Models;
using FormBuilder.Core.Notifications;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Serialization;

using Umbraco.Cms.Core.Services;

using static FormBuilder.Constants;

namespace FormBuilder.Core.CacheRefresher
{
    internal sealed class WorkflowCacheRefresher(
      AppCaches appCache,
      IJsonSerializer serializer,
      IEventAggregator eventAggregator,
      ICacheRefresherNotificationFactory factory,
      IRuntimeState runtimeState) :
        PayloadCacheRefresherBase<WorkflowCacheRefresherNotification, JsonPayloads.WorkflowPayload>(appCache, serializer, eventAggregator, factory)
    {
        private readonly IRuntimeState _runtimeState = runtimeState;

        public override Guid RefresherUniqueId => FormBuilderCacheKeys.WorkflowDbCacheRefresherGuid;

        public override string Name => "Umbraco Forms Workflow Cache Refresher";
    }
}