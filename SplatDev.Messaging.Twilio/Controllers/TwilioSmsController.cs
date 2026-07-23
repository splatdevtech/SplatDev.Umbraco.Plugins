namespace SplatDev.Messaging.Twilio.Controllers
{
    using global::Twilio;
    using global::Twilio.Rest.Api.V2010.Account;
    using global::Twilio.Types;

    using Microsoft.Win32.SafeHandles;

    using SplatDev.Messaging.Interfaces;
    using SplatDev.Messaging.Twilio.Models;

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public class TwilioSmsController : IMessagingController<Sms, MessageResource>
    {
        public TwilioSmsController(string accountSid, string authToken)
        {
            TwilioClient.Init(accountSid, authToken);
        }

        /// <summary>
        /// Sends the Sms message.
        /// </summary>
        /// <param name="message">The Sms message.</param>
        /// <returns>Sms response from the Twilio service</returns>
        public MessageResource SendMessage(Sms message)
        {
            MessageResource msg = MessageResource.Create(
                body: message.Body,
                from: message.From,
                to: message.To
            );
            return msg;
        }

        /// <summary>
        /// Sends the Sms message.
        /// </summary>
        /// <param name="subject">NOT USED</param>
        /// <param name="from">NOT USED</param>
        /// <param name="fromAddress">The Phone number which originated the message</param>
        /// <param name="to">NOT USED</param>
        /// <param name="toAddress">The phone number recipient of the message</param>
        /// <param name="message">NOT USED</param>
        /// <param name="plainMessage">The Sms message being sent.</param>
        /// <param name="bcc">NOT USED</param>
        /// <param name="cc">NOT USED</param>
        /// <returns>Sms response from the Twilio service</returns>
        public MessageResource SendMessage(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            MessageResource msg = MessageResource.Create(
                body: plainMessage,
                from: new PhoneNumber(fromAddress),
                to: new PhoneNumber(toAddress)
            );
            return msg;
        }

        /// <summary>
        /// Sends the message asynchronous.
        /// </summary>
        /// <param name="message">The Sms message.</param>
        /// <returns>Sms response from the Twilio service</returns>
        public async Task<MessageResource> SendMessageAsync(Sms message)
        {
            MessageResource msg = await MessageResource.CreateAsync(
                 body: message.Body,
                 from: message.From,
                 to: message.To
             );
            return msg;
        }

        /// <summary>
        /// Sends the Sms message asynchronous.
        /// </summary>
        /// <param name="subject">NOT USED</param>
        /// <param name="from">NOT USED</param>
        /// <param name="fromAddress">The Phone number which originated the message</param>
        /// <param name="to">NOT USED</param>
        /// <param name="toAddress">The phone number recipient of the message</param>
        /// <param name="message">NOT USED</param>
        /// <param name="plainMessage">The Sms message being sent.</param>
        /// <param name="bcc">NOT USED</param>
        /// <param name="cc">NOT USED</param>
        /// <returns>Sms response from the Twilio service</returns>
        public async Task<MessageResource> SendMessageAsync(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            MessageResource msg = await MessageResource.CreateAsync(
                body: plainMessage,
                from: new PhoneNumber(fromAddress),
                to: new PhoneNumber(toAddress)
            );
            return msg;
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

        ~TwilioSmsController() => Dispose(false);
        #endregion
    }
}
