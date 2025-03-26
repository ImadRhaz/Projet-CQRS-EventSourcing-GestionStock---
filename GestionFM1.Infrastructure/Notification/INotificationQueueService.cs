using System.Threading.Tasks;
using GestionFM1.Infrastructure.Notification;

namespace GestionFM1.Infrastructure.Notification
{
    public interface INotificationQueueService
    {
        Task EnqueueNotificationAsync(NotificationEvent notification);
    }
}