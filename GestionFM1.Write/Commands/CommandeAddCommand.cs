using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Write.Commands
{
    public class CommandeAddCommand : ICommand // <-- Renommer ici
    {
        public string EtatCommande { get; set; } = string.Empty;
        public DateTime DateCmd { get; set; }
        public Guid ComposentId { get; set; }
        public string ExpertId { get; set; }
        public string RaisonDeCommande { get; set; } = string.Empty;
        public Guid FM1Id { get; set; }

                public Guid? FM1HistoryId { get; set; }

    }
}