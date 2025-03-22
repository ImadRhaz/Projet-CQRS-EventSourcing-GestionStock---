using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.Core.Models
{
    public class ExcelComposent
    {
        [Key]
        public Guid Id { get; set; } // Utilisation de Guid comme clé primaire

        [Required]
        public string AnComposent { get; set; } = string.Empty;

        [Required]
        public string ComposentName { get; set; } = string.Empty;

        [Required]
        public string SnComposent { get; set; } = string.Empty;

        [Required]
        public double TotalAvailable { get; set; }
    }
}