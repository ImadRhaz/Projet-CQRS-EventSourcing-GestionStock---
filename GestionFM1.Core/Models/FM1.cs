using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Core.Models
{
    public class FM1
    {
        [Key]
        public Guid Id { get; set; }
        public string CodeSite { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string PsSn { get; set; } = string.Empty;
        public DateTime? DateEntre { get; set; }
        public DateTime? ExpirationVerification { get; set; }
        public string Status { get; set; } = string.Empty;

        // Clé étrangère vers l'expert (User)
        public string? ExpertId { get; set; }
        [ForeignKey("ExpertId")]
        public User? Expert { get; set; }

        // Relation One-to-Many : Un FM1 peut avoir plusieurs Composents
        public List<Composent> Composents { get; set; } = new List<Composent>();
        public List<Commande> Commandes { get; set; } = new List<Commande>(); // Relation avec Commandes


        // Relation One-to-One : Un FM1 a un FM1History (et vice-versa)
        public Guid? FM1HistoryId { get; set; }
        public FM1History? FM1History { get; set; }
    }
}