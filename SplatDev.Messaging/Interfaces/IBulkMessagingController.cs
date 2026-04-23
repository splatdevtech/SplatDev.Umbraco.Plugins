namespace SplatDev.Messaging.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBulkMessagingController<T, U> : IDisposable where T : class where U : class
    {
        U BulkSendMessage(IEnumerable<IBulkAddress> to, string from, string subject, string message, string plainMessage);
        U BulkSendMessage(T message, IEnumerable<IBulkAddress> to = null);

        Task<U> BulkSendMessageAsync(IEnumerable<IBulkAddress> to, string from, string subject, string message, string plainMessage);
        Task<U> BulkSendMessageAsync(T message, IEnumerable<IBulkAddress> to = null);
    }
}
