using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.Write.EventStore
{
    public class EventEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string AggregateId { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}