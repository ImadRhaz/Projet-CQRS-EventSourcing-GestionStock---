using GestionFM1.Core.Events;
using System;
using System.Collections.Generic;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Aggregates
{
    public class FM1Aggregate
    {
        public Guid FM1Id { get; private set; }
        public string CodeSite { get; private set; } = string.Empty;
        public string DeviceType { get; private set; } = string.Empty;
        public string PsSn { get; private set; } = string.Empty;
        public DateTime? DateEntre { get; private set; }
        public DateTime? ExpirationVerification { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public string? ExpertId { get; private set; }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public FM1Aggregate()
        {
        }

        public void AddFM1(
            Guid fm1Id,
            string codeSite,
            string deviceType,
            string psSn,
            DateTime? dateEntre,
            DateTime? expirationVerification,
            string status,
            string? expertId)
        {
            Apply(new FM1CreatedEvent
            {
                FM1Id = fm1Id,
                CodeSite = codeSite,
                DeviceType = deviceType,
                PsSn = psSn,
                DateEntre = dateEntre,
                ExpirationVerification = expirationVerification,
                Status = status,
                ExpertId = expertId
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