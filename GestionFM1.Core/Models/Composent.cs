using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Core.Models
{
    public class Composent
    {
        [Key]
        public Guid Id { get; set; }

        public int ItemBaseId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? SN { get; set; }

        public int TotalAvailable { get; set; }

        public string UrgentOrNot { get; set; } = string.Empty;

        public string? OrderOrNot { get; set; }

        // Clé étrangère vers FM1
        public Guid FM1Id { get; set; }
        [ForeignKey("FM1Id")]
        public FM1 FM1 { get; set; } = null!; // Propriété de navigation

        // Clé étrangère vers Commande
        public int? CommandeId { get; set; }  // Clé étrangère nullable (un composant peut ne pas avoir de commande)
        [ForeignKey("CommandeId")]
        public Commande? Commande { get; set; }
    }
}