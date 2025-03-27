using System;
using System.ComponentModel.DataAnnotations;

namespace GestionFM1.DTOs
{
    public class AddFM1HistoryDTO
    {
        [Required]
        public Guid FM1Id { get; set; }
    }
}