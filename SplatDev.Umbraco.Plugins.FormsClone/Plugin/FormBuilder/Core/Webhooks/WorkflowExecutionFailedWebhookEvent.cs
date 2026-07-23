using FormBuilder.Core.Enums;
using FormBuilder.Core.Services.Notifications;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Configuration.Models;

using Umbraco.Cms.Core.Services;

using Umbraco.Cms.Core.Sync;

using Umbraco.Cms.Core.Webhooks;

namespace FormBuilder.Core.Webhooks
{
    [WebhookEvent("Forms workflow execution failed", "FormBuilderTask")]
    public class
        WorkflowExecutionFailedWebhookEvent(
      IWebhookFiringService webhookFiringService,
      IWebhookService webhookService,
      IOptionsMonitor<WebhookSettings> webhookSettings,
      IServerRoleAccessor serverRoleAccessor) :
      WorkflowExecutionWebhookEventBase<WorkflowExecutionFailedNotification>(webhookFiringService, webhookService, webhookSettings, serverRoleAccessor)
    {
        protected override WorkflowExecutionStatus WorkflowExecutionStatus => WorkflowExecutionStatus.Failed;

        public override string Alias => "FormBuilderWorkflowExecutionFailed";
    }
}