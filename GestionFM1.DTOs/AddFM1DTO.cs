using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{
    public class AddFM1DTO
    {
        [Required]
        public string CodeSite { get; set; } = string.Empty;

        [Required]
        public string DeviceType { get; set; } = string.Empty;

        [Required]
        public string PsSn { get; set; } = string.Empty;

        public DateTime? DateEntre { get; set; }

        public DateTime? ExpirationVerification { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? ExpertId { get; set; }
    }
}