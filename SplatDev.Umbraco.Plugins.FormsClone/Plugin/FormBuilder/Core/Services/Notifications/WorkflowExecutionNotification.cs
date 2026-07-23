using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

using Umbraco.Cms.Core.Notifications;

namespace FormBuilder.Core.Services.Notifications
{
    public abstract class WorkflowExecutionNotification(Workflow target, Record record, EventMessages messages) : ObjectNotification<Workflow>(target, messages)
    {
        public Workflow Workflow => Target;

        public Record Record { get; } = record;
    }
}