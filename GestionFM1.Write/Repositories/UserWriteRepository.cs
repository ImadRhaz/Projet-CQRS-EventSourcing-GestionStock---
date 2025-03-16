using GestionFM1.Write.Commands; // <-- Choisir celui-ci
using GestionFM1.Write.Aggregates;
using GestionFM1.Write.EventStore;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Repositories;

public class UserWriteRepository
{
    private readonly IEventStore _eventStore;

    public UserWriteRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task RegisterUserAsync(RegisterUserCommand command)
    {
        var aggregate = new UserAggregate();
        aggregate.RegisterUser(Guid.NewGuid().ToString(), command.Email, command.Nom, command.Prenom);

        foreach (var @event in aggregate.GetChanges())
        {
            await _eventStore.SaveEventAsync(@event);
        }
    }
}