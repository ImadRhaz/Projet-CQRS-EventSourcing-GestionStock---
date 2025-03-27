using System;

namespace GestionFM1.Infrastructure.Data
{
  public class Notification
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int? CommandeId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string CommandeStatus { get; set; } = string.Empty; // Nouveau champ
}
}