using System;

namespace GestionFM1.DTOs  // Or your preferred namespace
{
    public class CommandeDetailsDTO
    {
        public int Id { get; set; }
        public string EtatCommande { get; set; } = string.Empty;
        public DateTime DateCmd { get; set; }

        public Guid ComposentId { get; set; }

        public string ExpertId { get; set; }
        public string ExpertNom { get; set; } = string.Empty;
        public string ExpertPrenom { get; set; } = string.Empty;

        public string RaisonDeCommande { get; set; } = string.Empty;

        public Guid FM1Id { get; set; }
        public Guid? FM1HistoryId { get; set; }

        public string ComposentProductName { get; set; } = string.Empty;
        public string? ComposentSN { get; set; }
        public string ComposentUrgentOrNot { get; set; } = string.Empty;
        public string? ComposentOrderOrNot { get; set; }

        public string FM1CodeSite { get; set; } = string.Empty;  // New property
        public string FM1DeviceType { get; set; } = string.Empty; // New property
        public string FM1PsSn { get; set; } = string.Empty;      // New property
    }
}