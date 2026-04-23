
// Type: Umbraco.Cms.Core.Webhooks.WorkflowExecutionWebhookEventBase`1
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Core.Webhooks;


#nullable enable
namespace Umbraco.Cms.Core.Webhooks
{
  public abstract class WorkflowExecutionWebhookEventBase<TNotification> : 
    WebhookEventBase<TNotification>
    where TNotification : WorkflowExecutionNotification
  {
    protected WorkflowExecutionWebhookEventBase(
      IWebhookFiringService webhookFiringService,
      IWebhookService webhookService,
      IOptionsMonitor<WebhookSettings> webhookSettings,
      IServerRoleAccessor serverRoleAccessor)
      : base(webhookFiringService, webhookService, webhookSettings, serverRoleAccessor)
    {
    }

    protected abstract WorkflowExecutionStatus WorkflowExecutionStatus { get; }

    public override object? ConvertNotificationToRequestPayload(TNotification notification) => (object) new WorkflowExecutionPayloadModel()
    {
      FormId = notification.Workflow.Form,
      RecordId = notification.Record.UniqueId,
      RecordState = notification.Record.State,
      WorkflowId = notification.Workflow.Id,
      WorkflowName = notification.Workflow.Name,
      Result = this.WorkflowExecutionStatus
    };
  }
}
