using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GestionFM1.Infrastructure.Data;
using NotificationData = GestionFM1.Infrastructure.Data.Notification;

namespace GestionFM1.Infrastructure.Notification
{
    public interface INotificationStorageService
    {
        Task SaveNotificationAsync(NotificationEvent notification);
        Task<IEnumerable<NotificationData>> GetUnreadNotificationsAsync(string userId);
        Task MarkAsReadAsync(Guid notificationId);
    }

    public class NotificationStorageService : INotificationStorageService
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<NotificationStorageService> _logger;

        public NotificationStorageService(
            NotificationDbContext context,
            ILogger<NotificationStorageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveNotificationAsync(NotificationEvent notification)
        {
            try
            {
                var entity = new NotificationData
                {
                    Id = notification.Id,
                    UserId = notification.UserId,
                    Title = notification.Title,
                    Message = notification.Message,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead,
                    CommandeId = notification.CommandeId,
                    NotificationType = notification.NotificationType
                };

                await _context.Notifications.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save notification");
                throw;
            }
        }

        public async Task<IEnumerable<NotificationData>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}