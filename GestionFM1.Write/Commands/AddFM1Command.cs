using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Commands
{
    public class AddFM1Command : ICommand
    {
        public string CodeSite { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string PsSn { get; set; } = string.Empty;
        public DateTime? DateEntre { get; set; }
        public DateTime? ExpirationVerification { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ExpertId { get; set; } // Facultatif
    }
}