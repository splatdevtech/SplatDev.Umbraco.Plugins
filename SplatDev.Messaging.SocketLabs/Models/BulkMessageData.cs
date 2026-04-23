namespace SplatDev.Messaging.SocketLabs.Models
{
    using SplatDev.Messaging.Interfaces;
    public class BulkMessageData : IBulkMessageData
    {
        public string Placeholder { get; set; }
        public string Value { get; set; }
    }
}
