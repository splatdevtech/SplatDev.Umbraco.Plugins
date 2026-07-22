namespace SplatDev.Tests
{
    using Xunit;

    using SplatDev.Messaging.Smtp.Controllers;

    public class Messaging
    {
        private readonly string subject = "Test Message";
        private readonly string fromEmail = "carlos@opennology.com";
        private readonly string from = "Carlos Casalicchio";
        private readonly string toEmail = "carlos.casalicchio@gmail.com";
        private readonly string to = "Carlos Casalicchio (Gmail)";

        [Fact(Skip = "Requires live SendGrid SDK credentials")]
        public void Messaging_SendGrid_Send()
        {
        }

        [Fact]
        public void Messaging_Smtp_Send()
        {
            var smtpController = new SmtpController();
            var msg = "<h1>This is a test with Html</h1>";

            smtpController.SendMessage(subject, from, fromEmail, to, toEmail, msg);

            Assert.False(false);
        }

        [Fact(Skip = "Requires live SocketLabs SDK credentials")]
        public void Messaging_SocketLabs_Send()
        {
        }

        [Fact(Skip = "Requires live SocketLabs SDK credentials")]
        public void Messaging_SocketLabs_BulkSend()
        {
        }

        [Fact(Skip = "Requires live Twilio SDK credentials")]
        public void Messaging_Twilio_Send()
        {
        }
    }
}
