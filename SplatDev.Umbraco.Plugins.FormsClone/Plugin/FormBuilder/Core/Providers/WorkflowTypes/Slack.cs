using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Options;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;

using System.Text;
using System.Text.Json;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    /// <remarks>
    /// This implementation supports only the newer method of Slack integration using webhooks.
    /// See:
    ///     https://slack.com/intl/en-it/help/articles/115005265063-Incoming-webhooks-for-Slack
    ///     https://api.slack.com/messaging/webhooks#posting_with_webhooks
    /// </remarks>
    public class Slack : BaseSlackWorkflowType
    {
        private readonly ILogger<Slack> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Slack(ILogger<Slack> logger, IHttpClientFactory httpClientFactory)
        {
            Id = new Guid("BC52AB28-D3FF-42EE-AF75-A5D49BE83040");
            Name = "Slack";
            Alias = "slack";
            Description = "Posts the form data to a specific channel on Slack using a webhook";
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>Gets or sets the webhook URL.</summary>
        [Setting("Webhook URL", Description = "Slack Webhook URL", DisplayOrder = 10, IsMandatory = true)]
        public virtual string WebhookUrl { get; set; } = string.Empty;

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            Slack slack = this;
            string content = JsonSerializer.Serialize(new
            {
                text = slack.GenerateNotificationMessage(context.Record, context.Form, false)
            }, FormsJsonSerializerOptions.Default);
            HttpClient client = slack._httpClientFactory.CreateClient();
            HttpRequestMessage request = new(HttpMethod.Post, new Uri(slack.WebhookUrl))
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            try
            {
                HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return WorkflowExecutionStatus.Completed;
                ILogger? logger = slack._logger;
                object? obj1 = httpResponseMessage.StatusCode;
                object? obj2 = httpResponseMessage.ReasonPhrase;
                string str = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError("There was a problem in the Slack (V2) workflow. Status code received from request: {StatusCode}, reason: {Reason}, content: {Content}", obj1, obj2, str);
                logger = null;
                obj1 = null;
                obj2 = null;
                return WorkflowExecutionStatus.Failed;
            }
            catch (Exception ex)
            {
                slack._logger.LogError(ex, "There was a problem in the Slack (V2) workflow");
                return WorkflowExecutionStatus.Failed;
            }
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(WebhookUrl))
                exceptionList.Add(new Exception("'Webhook URL' setting has not been set"));
            return exceptionList;
        }
    }
}