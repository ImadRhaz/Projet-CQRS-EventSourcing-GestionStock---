using System;

namespace GestionFM1.Infrastructure.Notification
{
    public class NotificationEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public int? CommandeId { get; set; }
        public string NotificationType { get; set; } = string.Empty;
    }
}