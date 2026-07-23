namespace SplatDev.Messaging.Interfaces
{
    using System.Collections.Generic;
    public interface IBulkAddress
    {
        string Name { get; set; }
        string Address { get; set; }
        IEnumerable<IBulkMessageData> Data { get; set; }
    }
}
