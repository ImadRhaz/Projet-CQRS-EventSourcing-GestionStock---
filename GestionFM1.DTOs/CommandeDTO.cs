using System;

namespace GestionFM1.Core.Models
{
    public class CommandeDTO
    {
        public int Id { get; set; }
        public string EtatCommande { get; set; } = string.Empty;
        public DateTime DateCmd { get; set; }
        public Guid ComposentId { get; set; }
        public string ExpertId { get; set; }
        public string RaisonDeCommande { get; set; } = string.Empty;
        public Guid FM1Id { get; set; }
        public Guid? FM1HistoryId { get; set; }
    }
}