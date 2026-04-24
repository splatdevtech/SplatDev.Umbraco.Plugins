using FormBuilder.Core.Enums;
using FormBuilder.Core.Services.Notifications;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;

using Umbraco.Cms.Core.Webhooks;

namespace FormBuilder.Core.Webhooks
{
    public abstract class WorkflowExecutionWebhookEventBase<TNotification>(
      IWebhookFiringService webhookFiringService,
      IWebhookService webhookService,
      IOptionsMonitor<WebhookSettings> webhookSettings,
      IServerRoleAccessor serverRoleAccessor) :
        WebhookEventBase<TNotification>(webhookFiringService, webhookService, webhookSettings, serverRoleAccessor)
        where TNotification : WorkflowExecutionNotification
    {
        protected abstract WorkflowExecutionStatus WorkflowExecutionStatus { get; }

        public override object? ConvertNotificationToRequestPayload(TNotification notification) => new WorkflowExecutionPayloadModel()
        {
            FormId = notification.Workflow.Form,
            RecordId = notification.Record.UniqueId,
            RecordState = notification.Record.State,
            WorkflowId = notification.Workflow.Id,
            WorkflowName = notification.Workflow.Name,
            Result = WorkflowExecutionStatus
        };
    }
}