using System;

namespace GestionFM1.DTOs
{
    public class ComposentDTO
    {
        public Guid Id { get; set; }
        public int ItemBaseId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SN { get; set; }
        public int TotalAvailable { get; set; }
        public string UrgentOrNot { get; set; } = string.Empty;
        public string? OrderOrNot { get; set; }
        public Guid FM1Id { get; set; }
        public int? CommandeId { get; set; }
   public string? SnDuComposentValidé { get; set; }
        public string? EtatCommande { get; set; } // New property to hold EtatCommande
    }
}