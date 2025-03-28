using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Core.Models
{
     public class Commande
    {
        [Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string EtatCommande { get; set; } = string.Empty;
        public DateTime DateCmd { get; set; }

     
        
    public Guid ComposentId { get; set; }
    public Composent Composent { get; set; }


        // Clé étrangère vers l'expert (User)
        public string ExpertId { get; set; }
        [ForeignKey("ExpertId")]
        public User Expert { get; set; } = null!; // L'expert qui a passé la commande

        public string RaisonDeCommande { get; set; } = string.Empty;

        // Clé étrangère vers FM1
        public Guid FM1Id { get; set; }
        [ForeignKey("FM1Id")]
        public FM1 FM1 { get; set; } = null!;  // FM1 auquel la commande est associée

        // Clé étrangère vers FM1History (nullable car une commande peut exister sans être liée à un historique)
        public Guid? FM1HistoryId { get; set; }
        [ForeignKey("FM1HistoryId")]
        public FM1History? FM1History { get; set; } // L'historique FM1 auquel la commande est liée (si applicable)
        public string? SnDuComposentValidé { get; set; } 
    }
}