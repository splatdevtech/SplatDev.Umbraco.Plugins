
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.Slack
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Collections.Specialized;
using System.Net;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class Slack : BaseSlackWorkflowType
    {
        private readonly ILogger<Slack> _logger;

        public Slack(ILogger<Slack> logger)
        {
            this.Id = new Guid("CCBFB0D5-ADAA-4729-8B4C-4BB439DC0202");
            this.Name = "Slack (Legacy)";
            this.Alias = "slackLegacy";
            this.Description = "Posts the form data to a specific channel on Slack using legacy tokens";
            this._logger = logger;
        }

        [Setting("API Token", Description = "Slack API token", DisplayOrder = 10, IsMandatory = true)]
        public virtual string Token { get; set; } = string.Empty;

        [Setting("Channel", Description = "Slack channel to post to", DisplayOrder = 20, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Channel { get; set; } = string.Empty;

        [Setting("Username", Description = "The username or bot to post as", DisplayOrder = 30, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Username { get; set; } = string.Empty;

        [Setting("Avatar URL", Description = "The full URL (including http/https) to the avatar image", DisplayOrder = 40, IsMandatory = true)]
        public virtual string AvatarUrl { get; set; } = string.Empty;

        public override Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            string notificationMessage = this.GenerateNotificationMessage(context.Record, context.Form);
            if (!this.Channel.StartsWith("#"))
                this.Channel = "#" + this.Channel;
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection data = new NameValueCollection()
        {
          {
            "channel",
            this.Channel
          },
          {
            "token",
            this.Token
          },
          {
            "username",
            this.Username
          },
          {
            "icon_url",
            this.AvatarUrl
          },
          {
            "text",
            notificationMessage
          }
        };
                try
                {
                    byte[] bytes = webClient.UploadValues("https://slack.com/api/chat.postMessage", "POST", data);
                    this._logger.LogDebug("Slack Workflow - Return API Value: {Response}", webClient.Encoding.GetString(bytes));
                    return Task.FromResult<WorkflowExecutionStatus>(WorkflowExecutionStatus.Completed);
                }
                catch (WebException ex)
                {
                    this._logger.LogError(ex, "There was a problem in the Slack (Legacy) workflow");
                    return Task.FromResult<WorkflowExecutionStatus>(WorkflowExecutionStatus.Failed);
                }
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.Token))
                exceptionList.Add(new Exception("'API Token' setting has not been set"));
            if (string.IsNullOrEmpty(this.Channel))
                exceptionList.Add(new Exception("'Channel' setting has not been set"));
            if (!this.Channel.StartsWith("#"))
                exceptionList.Add(new Exception("'Channel' is missing # at beginning"));
            if (string.IsNullOrEmpty(this.Username))
                exceptionList.Add(new Exception("'Username' setting has not been set"));
            if (string.IsNullOrEmpty(this.AvatarUrl))
                exceptionList.Add(new Exception("'Avatar URL' setting has not been set"));
            return exceptionList;
        }
    }
}
