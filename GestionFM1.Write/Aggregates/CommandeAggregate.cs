using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionFM1.Write.Aggregates
{
    public class CommandeAggregate
    {
        public string EtatCommande { get; private set; } = string.Empty;
        public DateTime DateCmd { get; private set; }
        public Guid ComposentId { get; private set; }
        public string ExpertId { get; private set; }
        public string RaisonDeCommande { get; private set; } = string.Empty;
        public Guid FM1Id { get; private set; }

        private readonly List<IEvent> _changes = new List<IEvent>();

        public CommandeAggregate()
        {
        }

        public void AddCommande(
            string etatCommande,
            DateTime dateCmd,
            Guid composentId,
            string expertId,
            string raisonDeCommande,
            Guid fm1Id
            )
        {
            Apply(new CommandeCreatedEvent
            {
                EtatCommande = etatCommande,
                DateCmd = dateCmd,
                ComposentId = composentId,
                ExpertId = expertId,
                RaisonDeCommande = raisonDeCommande,
                FM1Id = fm1Id
            });
        }

        internal void Apply(IEvent @event)
        {
            _changes.Add(@event);
            When(@event);
        }

        public IEnumerable<IEvent> GetChanges()
        {
            return _changes;
        }

        public void Load(IEnumerable<IEvent> history)
        {
            foreach (var @event in history)
            {
                When(@event);
            }
        }

        private void When(IEvent @event)
        {
            switch (@event)
            {
                case CommandeCreatedEvent e:
                    ComposentId = e.ComposentId;
                    EtatCommande = e.EtatCommande;
                    DateCmd = e.DateCmd;
                    ExpertId = e.ExpertId;
                    RaisonDeCommande = e.RaisonDeCommande;
                    FM1Id = e.FM1Id;
                    break;
            }
        }
    }
}