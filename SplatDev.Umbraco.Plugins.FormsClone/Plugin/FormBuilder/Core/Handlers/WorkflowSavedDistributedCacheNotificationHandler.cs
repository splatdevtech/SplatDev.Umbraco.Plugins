using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class WorkflowSavedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        SavedDistributedCacheNotificationHandlerBase<Workflow, WorkflowSavedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<Workflow> entities) => _formsDbDistributedCache.RefreshWorkflows(entities);
    }
}