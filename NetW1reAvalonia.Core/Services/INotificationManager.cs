using NetW1reAvalonia.Core.Services.Implementations.Notifications;

namespace NetW1reAvalonia.Core.Services
{
    public interface INotificationManager
    {
        void SendNotification();
        void ClearNotifications();
        void DestroyService();
    }
}
