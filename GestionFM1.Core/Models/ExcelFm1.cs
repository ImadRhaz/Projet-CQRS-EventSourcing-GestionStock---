using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.Core.Models
{
    public class ExcelFm1
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string SiteCode { get; set; } = string.Empty;

        [Required]
        public string TypeDevice { get; set; } = string.Empty;

        [Required]
        public string SnPs { get; set; } = string.Empty;
    }
}