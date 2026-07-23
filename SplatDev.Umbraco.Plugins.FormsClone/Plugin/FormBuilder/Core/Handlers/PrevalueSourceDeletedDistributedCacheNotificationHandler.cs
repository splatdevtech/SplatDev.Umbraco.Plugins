using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class PrevalueSourceDeletedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        DeletedDistributedCacheNotificationHandlerBase<FieldPrevalueSource, PrevalueSourceDeletedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<FieldPrevalueSource> entities) => _formsDbDistributedCache.RemovePrevalueSources(entities);
    }
}