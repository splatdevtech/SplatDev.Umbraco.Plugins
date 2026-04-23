
// Type: Umbraco.Forms.Core.Extensions.WebhookEventCollectionBuilderExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Webhooks;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class WebhookEventCollectionBuilderExtensions
  {
    public static WebhookEventCollectionBuilder AddForms(
      this WebhookEventCollectionBuilder builder1,
      bool onlyDefault = false)
    {
      return builder1.AddForms((Action<WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms>) (builder2 =>
      {
        if (onlyDefault)
          builder2.AddDefault();
        else
          builder2.AddWorkflowExecution();
      }));
    }

    public static WebhookEventCollectionBuilder AddForms(
      this WebhookEventCollectionBuilder builder,
      Action<WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms> formsBuilder)
    {
      formsBuilder(new WebhookEventCollectionBuilderExtensions.WebhookEventCollectionBuilderForms(builder));
      return builder;
    }

    public sealed class WebhookEventCollectionBuilderForms
    {
      internal WebhookEventCollectionBuilderForms(WebhookEventCollectionBuilder builder) => this.Builder = builder;

      internal WebhookEventCollectionBuilder Builder { get; }
    }
  }
}
