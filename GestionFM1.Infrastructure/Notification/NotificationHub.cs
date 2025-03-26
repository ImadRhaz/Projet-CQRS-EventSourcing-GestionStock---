using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace GestionFM1.Infrastructure.Notification
{
    public class NotificationHub : Hub
    {
        public async Task JoinNotificationGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task MarkAsRead(Guid notificationId)
        {
            await Clients.Caller.SendAsync("NotificationRead", notificationId);
        }
    }
}