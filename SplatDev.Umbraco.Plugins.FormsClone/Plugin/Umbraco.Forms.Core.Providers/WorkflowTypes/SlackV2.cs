
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.SlackV2
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Text;
using System.Text.Json;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class SlackV2 : BaseSlackWorkflowType
    {
        private readonly ILogger<SlackV2> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SlackV2(ILogger<SlackV2> logger, IHttpClientFactory httpClientFactory)
        {
            this.Id = new Guid("BC52AB28-D3FF-42EE-AF75-A5D49BE83040");
            this.Name = "Slack";
            this.Alias = "slack";
            this.Description = "Posts the form data to a specific channel on Slack using a webhook";
            this._logger = logger;
            this._httpClientFactory = httpClientFactory;
        }

        [Setting("Webhook URL", Description = "Slack Webhook URL", DisplayOrder = 10, IsMandatory = true)]
        public virtual string WebhookUrl { get; set; } = string.Empty;

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            SlackV2 slackV2 = this;
            string content = JsonSerializer.Serialize(new
            {
                text = slackV2.GenerateNotificationMessage(context.Record, context.Form, false)
            }, FormsJsonSerializerOptions.Default);
            HttpClient client = slackV2._httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(slackV2.WebhookUrl))
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            try
            {
                HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return WorkflowExecutionStatus.Completed;
                ILogger logger = slackV2._logger;
                object obj1 = httpResponseMessage.StatusCode;
                object obj2 = httpResponseMessage.ReasonPhrase;
                string str = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError("There was a problem in the Slack (V2) workflow. Status code received from request: {StatusCode}, reason: {Reason}, content: {Content}", obj1, obj2, str);
                logger = null;
                obj1 = null;
                obj2 = null;
                return WorkflowExecutionStatus.Failed;
            }
            catch (Exception ex)
            {
                slackV2._logger.LogError(ex, "There was a problem in the Slack (V2) workflow");
                return WorkflowExecutionStatus.Failed;
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.WebhookUrl))
                exceptionList.Add(new Exception("'Webhook URL' setting has not been set"));
            return exceptionList;
        }
    }
}
