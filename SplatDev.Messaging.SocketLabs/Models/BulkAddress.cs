namespace SplatDev.Messaging.SocketLabs.Models
{
    using SplatDev.Messaging.Interfaces;

    using System.Collections.Generic;

    public class BulkAddress : IBulkAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<IBulkMessageData> Data { get; set; }
    }
}
