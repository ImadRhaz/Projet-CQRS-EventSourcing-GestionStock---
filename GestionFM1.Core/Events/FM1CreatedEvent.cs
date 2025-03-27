using GestionFM1.Core.Interfaces;

namespace GestionFM1.Core.Events
{
    public class FM1CreatedEvent : IEvent
    {
        public Guid FM1Id { get; set; }
        public string CodeSite { get; set; }
        public string DeviceType { get; set; }
        public string PsSn { get; set; }
        public DateTime? DateEntre { get; set; }
        public DateTime? ExpirationVerification { get; set; }
        public string Status { get; set; }
        public string ExpertId { get; set; }
    }
}