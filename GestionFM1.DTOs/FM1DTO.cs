using System;
using GestionFM1.DTOs;

namespace GestionFM1.Core.Models
{
    public class FM1DTO
    {
        public Guid Id { get; set; }
        public string CodeSite { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string PsSn { get; set; } = string.Empty;
        public DateTime? DateEntre { get; set; }
        public DateTime? ExpirationVerification { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ExpertId { get; set; }
        public Guid? FM1HistoryId { get; set; }
                public UserDTO? Expert { get; set; }
                public List<ComposentDTO> Composents { get; set; } = new List<ComposentDTO>();
         public List<CommandeDTO> Commandes { get; set; } = new List<CommandeDTO>(); 

    }
     
}