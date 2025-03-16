using System;

namespace GestionFM1.Write.EventStore;

public class Event
{
    public Guid Id { get; set; }
    public string AggregateId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}