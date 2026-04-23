namespace SplatDev.Messaging.Twilio.Models
{
    using global::Twilio.Types;
    public class Sms
    {
        public string Body { get; set; }
        public PhoneNumber From { get; set; }
        public PhoneNumber To { get; set; }
    }
}
