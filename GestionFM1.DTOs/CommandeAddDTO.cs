using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{
    public class CommandeAddDTO
    {
        [Required]
        public string EtatCommande { get; set; } = string.Empty;

        [Required]
        public DateTime DateCmd { get; set; }

        [Required]
        public Guid ComposentId { get; set; }

        [Required]
        public string ExpertId { get; set; }

        [Required]
        public string RaisonDeCommande { get; set; } = string.Empty;

        [Required]
        public Guid FM1Id { get; set; }
    }
}