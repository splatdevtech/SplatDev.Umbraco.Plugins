namespace SplatDev.ScheduledTasks.Events
{
    using SplatDev.ScheduledTasks.Enums;
    using SplatDev.ScheduledTasks.Models;

    using System;
    using System.Threading;

    public class ScheduledEventArgs : EventArgs
    {
        public ScheduledEventArgs() { }
        public ScheduledEventArgs(string message, ScheduledMessageType messageType, ScheduledTaskPayload payload, AutoResetEvent autoReset = null)
        {
            Message = message;
            MessageType = messageType;
            Payload = payload ?? new ScheduledTaskPayload();
            AutoReset = autoReset;
        }

        public AutoResetEvent AutoReset { get; set; } = null;
        public string Message { get; set; } = string.Empty;
        public ScheduledMessageType MessageType { get; set; } = ScheduledMessageType.Info;
        public ScheduledTaskPayload Payload { get; set; }
    }
}
