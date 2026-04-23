
// Type: Umbraco.Forms.Core.Extensions.WebhookEventCollectionBuilderFormsExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Webhooks;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class WebhookEventCollectionBuilderFormsExtensions
  {
    private static readonly Type[] _defaultTypes = new Type[3]
    {
      typeof (WorkflowExecutionCancelledWebhookEvent),
      typeof (WorkflowExecutionCompletedWebhookEvent),
      typeof (WorkflowExecutionFailedWebhookEvent)
    };

    public static WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms AddDefault(
      this WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms builder)
    {
      builder.Builder.Add((IEnumerable<Type>) WebhookEventCollectionBuilderFormsExtensions._defaultTypes);
      return builder;
    }

    public static WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms RemoveDefault(
      this WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms builder)
    {
      foreach (Type defaultType in WebhookEventCollectionBuilderFormsExtensions._defaultTypes)
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
