namespace SplatDev.Tests
{
    using Xunit;

    using SocketLabs.InjectionApi.Message;

    using SplatDev.Messaging.SendGrid.Controllers;
    using SplatDev.Messaging.Smtp.Controllers;
    using SplatDev.Messaging.Smtp.Models;
    using SplatDev.Messaging.SocketLabs.Controllers;
    using SplatDev.Messaging.SocketLabs.Models;
    using SplatDev.Messaging.Models;
    using SplatDev.Messaging.Twilio.Controllers;
    using SplatDev.Messaging.Twilio.Models;

    using System.Configuration;

    public class Messaging
    {
        private readonly string subject = "Test Message";
        private readonly string fromEmail = "carlos@opennology.com";
        private readonly string from = "Carlos Casalicchio";
        private readonly string toEmail = "carlos.casalicchio@gmail.com";
        private readonly string to = "Carlos Casalicchio (Gmail)";

        [Fact]
        public void Messaging_SendGrid_Send()
        {
            // Arrange
            var apiKey = ConfigurationManager.AppSettings["SendGrid.Api"];
            var sendGridController = new SendGridController(apiKey);
            var msg = "<h1>This is a test with Html</h1>";
            var plainMsg = "this is a plain text message";
            // Act
            var response = sendGridController.SendMessageAsync(subject, from, fromEmail, to, toEmail, msg, plainMsg).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(response.StatusCode, System.Net.HttpStatusCode.Accepted);
        }

        [Fact]
        public void Messaging_Smtp_Send()
        {
            // Arrange
            var smtpController = new SmtpController(new SmtpOptions
            {
                Host = ConfigurationManager.AppSettings["Smtp.Host"] ?? "localhost",
                Port = int.TryParse(ConfigurationManager.AppSettings["Smtp.Port"], out var p) ? p : 587,
                User = ConfigurationManager.AppSettings["Smtp.User"],
                Password = ConfigurationManager.AppSettings["Smtp.Password"],
            });
            var msg = "<h1>This is a test with Html</h1>";

            // Act
            smtpController.SendMessage(subject, from, fromEmail, to, toEmail, msg);

            // Assert
            Assert.False(false);
        }

        [Fact]
        public void Messaging_SocketLabs_Send()
        {
            // Arrange
            var socketLabsController = new SocketLabsController(40684, "SOCKETLABS_API_KEY_REMOVED");
            var message = new BasicMessage
            {
                Subject = "Sending A Basic Message",
                HtmlBody = "<html>This is the Html Body of my message.</html>",
                PlainTextBody = "This is the Plain Text Body of my message."
            };
            message.From.Email = fromEmail;
            message.To.Add(new EmailAddress(toEmail, to));


            // Act
            var response = socketLabsController.SendMessage(message);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void Messaging_SocketLabs_BulkSend()
        {
            // Arrange
            var socketLabsController = new SocketLabsBulkController(40684, "SOCKETLABS_API_KEY_REMOVED");
            BulkAddress[] addresses = new BulkAddress[] {
                new BulkAddress{ Name = "Carlos1", Address = "carlos.casalicchio@gmail.com"},
                new BulkAddress{ Name = "Carlos2", Address = "carlos@opennology.com"},
                new BulkAddress{ Name = "Carlos3", Address = "carlos.casalicchio@splatdev.com"},
                new BulkAddress{ Name = "Carlos4", Address = "carlos.casalicchio@outlook.com"},
                new BulkAddress{ Name = "Carlos5", Address = "elder_casalicchio@hotmail.com"}
            };

            for (int i = 0; i < addresses.Length; i++)
            {
                addresses[i].Data = new BulkMessageData[] {
                    new BulkMessageData { Placeholder = "Name", Value = addresses[i].Name },
                    new BulkMessageData { Placeholder = "Email", Value = addresses[i].Address }
               };
            }

            var message = new BulkMessage
            {
                PlainTextBody = "This is the body of my message sent to %%Name%%",
                HtmlBody = "<html>This is the <strong>HtmlBody</strong> of my message sent to %%Name%%<br/><a href='mailto:%%Email%%'>%%Email%%</a></html>",
                Subject = "Sending a Bulk Message"
            };
            message.From.Email = fromEmail;


            // Act
            var response = socketLabsController.BulkSendMessage(message, addresses);

            // Assert
            Assert.NotNull(response);
        }

        [Fact]
        public void Messaging_Twilio_Send()
        {
            // Arrange
            var twilio = new TwilioSmsController(new TwilioOptions
            {
                AccountSid = ConfigurationManager.AppSettings["Twilio.AccountSid"] ?? "TWILIO_ACCOUNT_SID_REMOVED",
                AuthToken = ConfigurationManager.AppSettings["Twilio.AuthToken"] ?? "TWILIO_AUTH_TOKEN_REMOVED",
            });

            // Act
            var response = twilio.SendMessage(new SplatDev.Messaging.Models.Sms
            {
                Body = "This is a c# test",
                From = "+19096374988",
                To = "+18017061898"
            });

            // Assert
            Assert.NotNull(response);
        }
    }
}
