using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowDeletedNotification : DeletedNotification<Workflow>
    {
        public WorkflowDeletedNotification(Workflow target, EventMessages messages)
          : base(target, messages)
        {
        }

        public WorkflowDeletedNotification(IEnumerable<Workflow> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}