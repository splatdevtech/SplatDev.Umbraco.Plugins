
// Type: Umbraco.Forms.Core.Webhooks.WorkflowExecutionFailedWebhookEvent
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core.Webhooks;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Services.Notifications;


#nullable enable
namespace Umbraco.Forms.Core.Webhooks
{
  [WebhookEvent("Forms workflow execution failed", "Umbraco.Forms.Task")]
  public class WorkflowExecutionFailedWebhookEvent : 
    WorkflowExecutionWebhookEventBase<WorkflowExecutionFailedNotification>
  {
    public WorkflowExecutionFailedWebhookEvent(
      IWebhookFiringService webhookFiringService,
      IWebhookService webhookService,
      IOptionsMonitor<WebhookSettings> webhookSettings,
      IServerRoleAccessor serverRoleAccessor)
      : base(webhookFiringService, webhookService, webhookSettings, serverRoleAccessor)
    {
    }

    protected override WorkflowExecutionStatus WorkflowExecutionStatus => WorkflowExecutionStatus.Failed;

    public override string Alias => "Umbraco.Forms.WorkflowExecutionFailed";
  }
}
