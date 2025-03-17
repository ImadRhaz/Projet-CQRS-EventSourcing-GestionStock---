using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Write.Commands
{
    public class AddComposentCommand : ICommand
    {
        public Guid ComposentId { get; set; }
        public int ItemBaseId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? SN { get; set; }
        public int TotalAvailable { get; set; }
        public string UrgentOrNot { get; set; } = string.Empty;
        public string? OrderOrNot { get; set; }
        public Guid FM1Id { get; set; }
    }
}