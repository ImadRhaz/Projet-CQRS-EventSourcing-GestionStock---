using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Core.Events
{
    public class FM1HistoryCreatedEvent : IEvent
    {
        public Guid FM1HistoryId { get; set; }
        public Guid FM1Id { get; set; }
    }
}