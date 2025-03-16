using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.Write.EventStore;

public class EventEntity
{
    [Key]
    public Guid Id { get; set; }
    public string AggregateId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
}