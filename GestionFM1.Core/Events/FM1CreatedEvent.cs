using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Core.Events
{
    public class FM1CreatedEvent : IEvent
    {
        public Guid FM1Id { get; set; }
        public string CodeSite { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string PsSn { get; set; } = string.Empty;
        public DateTime? DateEntre { get; set; }
        public DateTime? ExpirationVerification { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ExpertId { get; set; }
    }
}