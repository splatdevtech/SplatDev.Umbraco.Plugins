using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowSavedNotification : SavedNotification<Workflow>
    {
        public WorkflowSavedNotification(Workflow target, EventMessages messages)
          : base(target, messages)
        {
        }

        public WorkflowSavedNotification(IEnumerable<Workflow> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}