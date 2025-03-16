using GestionFM1.Core.Events;
using System.Collections.Generic;
using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Write.Aggregates;

public class UserAggregate
{
    public string UserId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Nom { get; private set; } = string.Empty;
    public string Prenom { get; private set; } = string.Empty;

    private readonly List<IEvent> _changes = new List<IEvent>();

    public UserAggregate()
    {
    }

    public void RegisterUser(string userId, string email, string nom, string prenom)
    {
        Apply(new UserCreatedEvent { UserId = userId, Email = email, Nom = nom, Prenom = prenom });
    }

    internal void Apply(IEvent @event)
    {
        _changes.Add(@event);
    }

    public IEnumerable<IEvent> GetChanges()
    {
        return _changes;
    }
}