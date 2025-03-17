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
        public User? Expert { get; set; }
        public string? ExpertId { get; set; }

        public List<Composent> Composents { get; set; } = new List<Composent>();
    }
}