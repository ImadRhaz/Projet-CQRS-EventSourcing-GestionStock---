using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Core.Models
{
    public class FM1History
    {
        [Key]
        public Guid Id { get; set; }

        // Clé étrangère vers FM1
        public Guid? FM1Id { get; set; }
        [ForeignKey("FM1Id")]
        public FM1? FM1 { get; set; } = null!;  // FM1 associé à cet historique

        // Relation One-to-Many : Un FM1History peut être le résultat de plusieurs Commandes
        public List<Commande> Commandes { get; set; } = new List<Commande>();
    }
}