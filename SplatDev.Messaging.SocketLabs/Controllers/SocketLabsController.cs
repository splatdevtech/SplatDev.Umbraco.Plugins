namespace SplatDev.Messaging.SocketLabs.Controllers
{
    using global::SocketLabs.InjectionApi;
    using global::SocketLabs.InjectionApi.Message;

    using Microsoft.Win32.SafeHandles;

    using SplatDev.Messaging.Interfaces;

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class SocketLabsController : IMessagingController<BasicMessage, SendResponse>
    {
        private readonly SocketLabsClient client;

        public SocketLabsController(int serverId, string injectionApiKey)
        {
            client = new SocketLabsClient(serverId, injectionApiKey);
        }

        public SendResponse SendMessage(BasicMessage message)
        {
            var response = client.Send(message);
            return response;
        }

        public SendResponse SendMessage(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            var msg = new BasicMessage
            {
                Subject = subject,
                HtmlBody = message,
                PlainTextBody = plainMessage
            };
            msg.From.Email = fromAddress;
            msg.To.Add(toAddress, to);
            return SendMessage(msg);
        }

        public async Task<SendResponse> SendMessageAsync(BasicMessage message)
        {
            var response = await client.SendAsync(message, CancellationToken.None).ConfigureAwait(false);
            return response;
        }

        public async Task<SendResponse> SendMessageAsync(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            var msg = new BasicMessage
            {
                Subject = subject,
                HtmlBody = message,
                PlainTextBody = plainMessage,
            };
            msg.From.Email = fromAddress;
            msg.To.Add(toAddress, to);
            var response = await client.SendAsync(msg, CancellationToken.None).ConfigureAwait(false);
            return response;
        }

        #region Dispose
        private bool _disposed = false;
        private readonly SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _safeHandle?.Dispose();
            }

            _disposed = true;
        }

        ~SocketLabsController() => Dispose(false);
        #endregion
    }
}
