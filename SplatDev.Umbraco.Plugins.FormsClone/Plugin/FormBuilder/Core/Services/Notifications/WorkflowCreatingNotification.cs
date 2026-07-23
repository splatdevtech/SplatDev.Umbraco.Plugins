using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowCreatingNotification(Workflow target, EventMessages messages) : CreatingNotification<Workflow>(target, messages)
    {
    }
}