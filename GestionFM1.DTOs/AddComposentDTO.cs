using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{
    public class AddComposentDTO
    {
        [Required]
        public int ItemBaseId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string? SN { get; set; }

        [Required]
        public int TotalAvailable { get; set; }

        [Required]
        public string UrgentOrNot { get; set; } = string.Empty;

        public string? OrderOrNot { get; set; }

        [Required]
        public Guid FM1Id { get; set; }
    }
}