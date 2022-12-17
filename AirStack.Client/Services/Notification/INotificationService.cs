using System;

namespace AirStack.Client.Services.Notification
{
    public interface INotificationProvider
    {
        void Notify(string message);
        void NotifyError(string message);
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}