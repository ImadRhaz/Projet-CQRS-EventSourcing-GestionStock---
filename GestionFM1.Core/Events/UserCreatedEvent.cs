using GestionFM1.Core.Interfaces;

namespace GestionFM1.Core.Events;

public class UserCreatedEvent : IEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}