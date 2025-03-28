using System;

namespace GestionFM1.DTOs
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

        // Nouvelles propriétés pour les informations du composant
        public string ComposantProductName { get; set; } = string.Empty;
        public string? ComposantSN { get; set; } //  ? si SN peut être null
        public string ComposantUrgentOrNot { get; set; } = string.Empty;
        public string? ComposantOrderOrNot { get; set; } // ? si OrderOrNot peut être null

        public string ExpertNom { get; set; } = string.Empty;
    public string ExpertPrenom { get; set; } = string.Empty;
    public string? SnDuComposentValidé { get; set; }
    

    }
}