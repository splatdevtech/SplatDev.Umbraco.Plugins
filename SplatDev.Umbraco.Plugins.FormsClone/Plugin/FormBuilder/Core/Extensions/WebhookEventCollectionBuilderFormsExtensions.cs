using FormBuilder.Core.Webhooks;

namespace FormBuilder.Core.Extensions
{
    public static class WebhookEventCollectionBuilderFormsExtensions
    {
        private static readonly Type[] _defaultTypes =
        [
              typeof (WorkflowExecutionCancelledWebhookEvent),
              typeof (WorkflowExecutionCompletedWebhookEvent),
              typeof (WorkflowExecutionFailedWebhookEvent)
        ];

        public static WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms AddDefault(
          this WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms builder)
        {
            builder.Builder.Add(_defaultTypes);
            return builder;
        }

        public static WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms RemoveDefault(
          this WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms builder)
        {
            foreach (Type defaultType in _defaultTypes)
                builder.Builder.Remove(defaultType);
            return builder;
        }

        public static WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms AddWorkflowExecution(
          this WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms builder)
        {
            builder.Builder.Add<WorkflowExecutionCancelledWebhookEvent>().Add<WorkflowExecutionCompletedWebhookEvent>().Add<WorkflowExecutionFailedWebhookEvent>();
            return builder;
        }
    }
}