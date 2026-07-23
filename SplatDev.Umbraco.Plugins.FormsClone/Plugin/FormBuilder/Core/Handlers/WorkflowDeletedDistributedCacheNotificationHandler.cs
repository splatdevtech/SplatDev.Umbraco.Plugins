using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Notifications;

using Umbraco.Cms.Core.Cache;

namespace FormBuilder.Core.Handlers
{
    internal sealed class WorkflowDeletedDistributedCacheNotificationHandler(
      IFormsDistributedCache formsDbDistributedCache) :
        DeletedDistributedCacheNotificationHandlerBase<Workflow, WorkflowDeletedNotification>
    {
        private readonly IFormsDistributedCache _formsDbDistributedCache = formsDbDistributedCache;

        protected override void Handle(IEnumerable<Workflow> entities) => _formsDbDistributedCache.RemoveWorkflows(entities);
    }
}