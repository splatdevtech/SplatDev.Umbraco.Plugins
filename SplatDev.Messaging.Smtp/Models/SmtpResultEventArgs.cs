namespace SplatDev.Messaging.Smtp.Models
{
    using System;
    public class SmtpResultEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
