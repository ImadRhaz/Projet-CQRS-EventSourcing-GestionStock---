using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace GestionFM1.Write.Aggregates
{
    public class FM1HistoryAggregate
    {
        public Guid FM1HistoryId { get; private set; }
        public Guid FM1Id { get; private set; }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public FM1HistoryAggregate()
        {
        }

        public void AddFM1History(Guid fm1HistoryId, Guid fm1Id)
        {
            Apply(new FM1HistoryCreatedEvent
            {
                FM1HistoryId = fm1HistoryId,
                FM1Id = fm1Id
            });
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
}