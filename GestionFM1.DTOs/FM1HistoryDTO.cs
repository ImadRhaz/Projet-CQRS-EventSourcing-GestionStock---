using System;
using System.Collections.Generic;

namespace GestionFM1.DTOs
{
    public class FM1HistoryDTO
    {
        public Guid Id { get; set; }
        public Guid? FM1Id { get; set; }

        public string FM1CodeSite { get; set; } = string.Empty;
        public string FM1DeviceType { get; set; } = string.Empty;
        public string FM1PsSn { get; set; } = string.Empty;

        // List of Commandes
        public List<CommandeDTO> Commandes { get; set; } = new List<CommandeDTO>();
    }
}