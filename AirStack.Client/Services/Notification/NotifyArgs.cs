using System;

namespace AirStack.Client.Services.Notification
{
    public class NotifyArgs : EventArgs
    {
        public NotifyArgs(string message, NotificationType notificationType)
        {
            Message = message;
            NotificationType = notificationType;
        }

        public string Message { get; }
        public NotificationType NotificationType { get; }
    }
}
