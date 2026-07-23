using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowDeletingNotification : DeletingNotification<Workflow>
    {
        public WorkflowDeletingNotification(Workflow target, EventMessages messages)
          : base(target, messages)
        {
        }

        public WorkflowDeletingNotification(IEnumerable<Workflow> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}