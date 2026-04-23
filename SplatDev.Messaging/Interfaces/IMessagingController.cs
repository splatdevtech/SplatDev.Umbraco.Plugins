namespace SplatDev.Messaging.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMessagingController<T, U> : IDisposable where T : class where U : class
    {
        U SendMessage(T message);
        U SendMessage(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null);

        Task<U> SendMessageAsync(T message);
        Task<U> SendMessageAsync(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null);
    }
}
