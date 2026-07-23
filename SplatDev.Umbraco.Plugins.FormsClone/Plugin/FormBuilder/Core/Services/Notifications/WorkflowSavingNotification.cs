using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowSavingNotification : SavingNotification<Workflow>
    {
        public WorkflowSavingNotification(Workflow target, EventMessages messages)
          : base(target, messages)
        {
        }

        public WorkflowSavingNotification(IEnumerable<Workflow> target, EventMessages messages)
          : base(target, messages)
        {
        }
    }
}