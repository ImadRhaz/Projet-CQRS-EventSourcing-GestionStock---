using GestionFM1.Core.Interfaces;
using System;

namespace GestionFM1.Write.Commands
{
    public class AddFM1HistoryCommand : ICommand
    {
        public Guid FM1Id { get; set; }
    }
}