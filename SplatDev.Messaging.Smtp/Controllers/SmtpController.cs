namespace SplatDev.Messaging.Smtp.Controllers
{
    using Microsoft.Win32.SafeHandles;

    using SplatDev.Messaging.Interfaces;
    using SplatDev.Messaging.Smtp.Models;

    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SmtpController : IMessagingController<MailMessage, SmtpResult>, IDisposable
    {
        private readonly SmtpClient client;

        public delegate void MailerSent(object sender, SmtpResultEventArgs args);
        public event MailerSent OnMailerSent;

        public SmtpController()
        {
            client = new SmtpClient();
        }

        public SmtpResult SendMessage(MailMessage message)
        {
            try
            {
                client.Send(message);
                var result = new SmtpResult { Success = true, Message = "Send Successful" };
                OnMailerSent?.Invoke(this, new SmtpResultEventArgs { Success = true, Message = "Send Successful" });
                return result;
            }
            catch (Exception ex)
            {
                var result = new SmtpResult { Success = false, Message = "Send failure", Exception = ex };
                OnMailerSent?.Invoke(this, new SmtpResultEventArgs { Success = false, Message = "Send failure", Exception = ex });
                return result;
            }
        }

        public SmtpResult SendMessage(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            MailMessage msg = new MailMessage(new MailAddress(fromAddress, from), new MailAddress(toAddress, to))
            {
                Body = message,
                IsBodyHtml = true,
                Subject = subject
            };

            if (cc != null)
            {
                foreach (var mail in cc)
                    msg.CC.Add(new MailAddress(mail.Address, mail.Name));
            }

            if (bcc != null)
            {
                foreach (var mail in bcc)
                    msg.Bcc.Add(new MailAddress(mail.Address, mail.Name));
            }
            return SendMessage(msg);
        }

        public async Task<SmtpResult> SendMessageAsync(MailMessage message)
        {
            var result = SendMessage(message);
            await Task.FromResult(0);
            return result;
        }

        public async Task<SmtpResult> SendMessageAsync(string subject, string from, string fromAddress, string to, string toAddress, string message, string plainMessage = "", IEnumerable<IAddress> bcc = null, IEnumerable<IAddress> cc = null)
        {
            var result = SendMessage(subject, from, fromAddress, to, toAddress, message, plainMessage, bcc, cc);
            await Task.FromResult(0);
            return result;
        }

        public void SendException(Exception exc, HttpContext? context, string from, string to, string applicationName)
        {
            if (string.IsNullOrEmpty(to)) throw new MissingFieldException("Missing send to field");

            var requestUrl = context?.Request.Path.Value ?? "unknown";
            var body = new System.Text.StringBuilder("<table>");
            body.Append("<tr><th>").Append($"An error occurred on {applicationName}").Append("</th></tr>");
            body.Append("<tr><td>");
            body.Append($"<h1>Url {requestUrl}</h1>");
            body.Append($"<h4>The error is {exc.Message}</h4><br/>");
            body.Append($"<h5>The Inner Exception is {exc.InnerException}</h5><br/>");
            body.Append("<h6>The StackTrace is </h6>");
            body.Append($"<pre>{exc.StackTrace}</pre>");
            body.Append("</td></tr></table>");
            string subject = $"Error in {applicationName} for url: {requestUrl}";
            SendMessage(subject, from, from, to, to, body.ToString());
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
                client.Dispose();
            }

            _disposed = true;
        }

        ~SmtpController() => Dispose(false);
        #endregion
    }
}
