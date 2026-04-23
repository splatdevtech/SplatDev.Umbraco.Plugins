using FormBuilder.Core.DataSources;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class DataSourceDeletedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        DeletedDistributedCacheNotificationHandlerBase<FormDataSource, DataSourceDeletedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<FormDataSource> entities) => _formsDbDistributedCache.RemoveDataSources(entities);
    }
}