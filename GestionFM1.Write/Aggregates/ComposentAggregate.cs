using GestionFM1.Core.Events;
using System;
using System.Collections.Generic;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Aggregates
{
    public class ComposentAggregate
    {
        public Guid ComposentId { get; private set; }
        public int ItemBaseId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string? SN { get; private set; }
        public int TotalAvailable { get; private set; }
        public string UrgentOrNot { get; private set; } = string.Empty;
        public string? OrderOrNot { get; private set; }
        public Guid FM1Id { get; private set; }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public ComposentAggregate()
        {
        }

        public void AddComposent(
            Guid composentId,
            int itemBaseId,
            string productName,
            string? sn,
            int totalAvailable,
            string urgentOrNot,
            string? orderOrNot,
            Guid fm1Id)
        {
            Apply(new ComposentCreatedEvent
            {
                ComposentId = composentId,
                ItemBaseId = itemBaseId,
                ProductName = productName,
                SN = sn,
                TotalAvailable = totalAvailable,
                UrgentOrNot = urgentOrNot,
                OrderOrNot = orderOrNot,
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