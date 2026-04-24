using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using Umbraco.Cms.Core.Events;

namespace FormBuilder.Core.Services.Notifications
{
    public sealed class WorkflowExecutionCancelledNotification(
      Workflow target,
      Record record,
      EventMessages messages) : WorkflowExecutionNotification(target, record, messages)
    {
    }
}