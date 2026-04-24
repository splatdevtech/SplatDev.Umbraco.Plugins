using Umbraco.Cms.Core.Webhooks;

namespace FormBuilder.Core.Extensions
{
    public static class WebhookEventCollectionBuilderExtensions
    {
        public static WebhookEventCollectionBuilder AddForms(
          this WebhookEventCollectionBuilder builder1,
          bool onlyDefault = false)
        {
            return builder1.AddForms(builder2 =>
            {
                if (onlyDefault)
                    builder2.AddDefault();
                else
                    builder2.AddWorkflowExecution();
            });
        }

        public static WebhookEventCollectionBuilder AddForms(
          this WebhookEventCollectionBuilder builder,
          Action<WebhookEventCollectionBuilderForms> formsBuilder)
        {
            formsBuilder(new WebhookEventCollectionBuilderForms(builder));
            return builder;
        }

        public sealed class WebhookEventCollectionBuilderForms
        {
            internal WebhookEventCollectionBuilderForms(WebhookEventCollectionBuilder builder) => Builder = builder;

            internal WebhookEventCollectionBuilder Builder { get; }
        }
    }
}