using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Core.Models
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }
        public string EtatCommande { get; set; } = string.Empty;
        public DateTime DateCmd { get; set; }

        // Clé étrangère vers Composent
        [ForeignKey("Composent")]  //Important : [ForeignKey] ici
        public Guid ComposentId { get; set; }
        public Composent Composent { get; set; } = null!; // Composent concerné par la commande
        
        // Clé étrangère vers l'expert (User)
        public string ExpertId { get; set; }
        [ForeignKey("ExpertId")]
        public User Expert { get; set; } = null!; // L'expert qui a passé la commande

        public string RaisonDeCommande { get; set; } = string.Empty;

        // Clé étrangère vers FM1
        public Guid FM1Id { get; set; }
        [ForeignKey("FM1Id")]
        public FM1 FM1 { get; set; } = null!;  // FM1 auquel la commande est associée
    }
}