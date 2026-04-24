using Azure.Identity;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

using System.Net.Mail;

using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using SplatDev.Umbraco.Plugins.Mailer.Extensions;
using SplatDev.Umbraco.Plugins.Mailer.Models;


namespace SplatDev.Umbraco.Plugins.Mailer.Services
{
    public class MicrosoftGraphMailerService : IEmailSender
    {
        private readonly SmtpSettings? _settings;
        private readonly string _tenant;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;

        public MicrosoftGraphMailerService(
            IConfiguration globalConfiguration,
            IWebHostEnvironment webHostEnvironment,
            ILogger<MicrosoftGraphMailerService> logger,
            IEventAggregator eventAggregator)
        {
            _settings = new SmtpSettings();
            globalConfiguration.GetSection("Umbraco:CMS:Global:Smtp").Bind(_settings);
            _tenant = globalConfiguration.GetValue<string>("Umbraco:CMS:Global:Smtp:TenantId") ?? "";
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _eventAggregator = eventAggregator;

            //_logger.LogInformation("Microsoft Graph Mailer Service\nTenant: {Tenant}, SMTP Host: {Host}, Port: {Port}",
            //    _tenant, _settings.Host, _settings.Port);
        }

        public bool CanSendRequiredEmail() => true;

        public async Task SendAsync(EmailMessage message, string emailType)
        {
            await SendAsync(message, emailType, false);
        }

        public async Task SendAsync(EmailMessage message, string emailType, bool enableNotification)
        {
            if (message is null) return;

            try
            {
                await SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Couldn't send email, check log details");
            }

            if (enableNotification && _settings is not null)
            {
                var notification =
                new SendEmailNotification(message.ToNotificationEmail(_settings.From), emailType);
                await _eventAggregator.PublishAsync(notification);

                // if a handler handled sending the email then don't continue.
                if (notification.IsHandled)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(
                        "The email sending for {Subject} was handled by a notification handler",
                        notification.Message.Subject);
                    }
                    return;
                }
            }
            _logger.LogInformation("Sent notification to {addresses}: \"{subject}\"", string.Join(", ", message.To), message.Subject);

        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            if (_settings is null) return;

            if (_settings.Host?.Equals("localhost") ?? false)
            {
                foreach (var to in message.To)
                {
                    MailMessage mailMessage = new(_settings.From, to!, message.Subject, message.Body);
                    new SmtpClient(_settings.Host, _settings.Port).Send(mailMessage);
                }
                return;
            }

            var clientId = _settings.Username;
            var clientSecret = _settings.Password;
            var tenantId = _tenant;
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Message
                {
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = message.Body
                    },
                    Subject = message.Subject,
                    ToRecipients = [.. message.To.Select(x =>
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = x
                            }
                        }
                    )],
                },
                SaveToSentItems = true
            };

            try
            {
                await graphClient.Users[_settings.From].SendMail.PostAsync(requestBody);
            }
            catch (Exception ex)
            {
                _ = ex;
                throw;
            }
        }

        public EmailMessage? GetMessage(
            string subject,
            string html,
            string? appendBody = null,
            Dictionary<string, string>? customValues = null,
            List<object>? sources = null,
            List<EmailAddress>? recipients = null,
            List<EmailAddress>? cc = null,
            List<EmailAddress>? bcc = null
            )
        {
            if (_settings is null) return null;

            var tokens = html.GetTokens();
            html = html.Replace("/%%url%%", "%%url%%");
            if (sources is not null && sources.Count > 0)
            {
                Dictionary<string, string> tokenValues = sources.GetTokenValues(tokens);
                html = html.ReplaceTokens(tokenValues);
            }

            if (customValues is not null)
            {
                html = html.ReplaceTokens(customValues);
            }

            if (!string.IsNullOrEmpty(appendBody))
            {
                html += appendBody;
            }
            try
            {
                var message = new EmailMessage(_settings.From,
                    to: recipients is not null && recipients.Count > 0 ? recipients.Select(x => x.Address).ToArray() : [""],
                    cc: cc is not null && cc.Count > 0 ? cc.Select(x => x.Address!).ToArray() : null,
                    bcc: bcc is not null && bcc.Count > 0 ? bcc.Select(x => x.Address!).ToArray() : null,
                    replyTo: null,
                    subject ?? "No Subject",
                    html,
                    isBodyHtml: true,
                    attachments: null);

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot send mail message");
                return null;
            }
        }


        public string GetMessageHtml(TemplateSource source, string? razorFilename = null, string? filePath = null, IPublishedContent? siteSettings = null, string? propertyAlias = null)
        {
            string html;
            switch (source)
            {
                case TemplateSource.Razor:
                    if (razorFilename is null) throw new MissingFieldException();
                    html = _webHostEnvironment.GetRazorHtml(razorFilename);
                    break;
                case TemplateSource.File:
                    if (filePath is null || razorFilename is null) throw new MissingFieldException();
                    html = _webHostEnvironment.GetFileHtml(filePath, razorFilename);
                    break;
                case TemplateSource.PropertyEditor:
                    if (propertyAlias is null) throw new MissingFieldException();
                    if (siteSettings is null) throw new MissingFieldException();
                    html = siteSettings.HasValue(propertyAlias) ? siteSettings.Value<string>(propertyAlias)! : string.Empty;
                    break;
                case TemplateSource.RawHtml:
                default:
                    html = string.Empty;
                    break;
            }
            return html;
        }

    }
}
