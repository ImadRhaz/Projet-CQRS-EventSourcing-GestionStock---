using GestionFM1.Core.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Write.EventStore;

public class EventStore : IEventStore
{
    private readonly EventStoreDbContext _eventStoreDbContext;
    private readonly ILogger<EventStore> _logger;

    public EventStore(EventStoreDbContext eventStoreDbContext, ILogger<EventStore> logger)
    {
        _eventStoreDbContext = eventStoreDbContext;
        _logger = logger;
    }

    public async Task SaveEventAsync(IEvent @event)
    {
        var eventType = @event.GetType().Name;
        var eventData = JsonConvert.SerializeObject(@event);
        string aggregateId;

        if (@event is UserCreatedEvent userCreatedEvent)
        {
            aggregateId = userCreatedEvent.UserId;
        }
        else if (@event is FM1CreatedEvent fm1CreatedEvent)
        {
            aggregateId = fm1CreatedEvent.FM1Id.ToString();
        }
        else if (@event is ComposentCreatedEvent composentCreatedEvent)
        {
            aggregateId = composentCreatedEvent.ComposentId.ToString();
        }
        else if (@event is CommandeCreatedEvent commandeCreatedEvent)
        {
            aggregateId = commandeCreatedEvent.ComposentId.ToString(); // Utiliser ComposentId
        }
        else
        {
            _logger.LogError($"Unsupported event type: {@event.GetType().Name}");
            throw new ArgumentException("Type d'événement non pris en charge", nameof(@event));
        }

        var eventToSave = new Event
        {
            Id = Guid.NewGuid(),
            AggregateId = aggregateId,
            EventType = eventType,
            EventData = eventData,
            Timestamp = DateTimeOffset.UtcNow
        };

        var eventEntity = new EventEntity
        {
            Id = eventToSave.Id,
            AggregateId = eventToSave.AggregateId,
            EventType = eventToSave.EventType,
            EventData = eventToSave.EventData,
            Timestamp = eventToSave.Timestamp
        };

        try
        {
            _eventStoreDbContext.Events.Add(eventEntity);
            await _eventStoreDbContext.SaveChangesAsync();
            _logger.LogInformation($"Event '{eventType}' saved to EventStore for aggregate '{aggregateId}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving event '{eventType}' to EventStore for aggregate '{aggregateId}'.");
            throw;
        }
    }

    public async Task<IEnumerable<IEvent>> LoadEventsAsync(string aggregateId)
    {
        var events = await _eventStoreDbContext.Events
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        var eventList = new List<IEvent>();

        foreach (var @event in events)
        {
            Type? eventType = Type.GetType($"GestionFM1.Core.Events.{@event.EventType}, GestionFM1.Core");

            if (eventType != null)
            {
                try
                {
                    object? eventObject = JsonConvert.DeserializeObject(@event.EventData, eventType);
                    if (eventObject is IEvent validEvent)
                    {
                        eventList.Add(validEvent);
                    }
                    else
                    {
                        _logger.LogWarning($"Deserialized object is not an IEvent: {@event.EventType}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"Error deserializing event {@event.EventType}: {ex.Message}");
                }
            }
            else
            {
                _logger.LogError($"Unable to find event type: {@event.EventType}");
            }
        }

        return eventList;
    }
}