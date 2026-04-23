using FormBuilder.Core.Models;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowCreatedNotification(Workflow target, EventMessages messages) : CreatedNotification<Workflow>(target, messages)
    {
    }
}