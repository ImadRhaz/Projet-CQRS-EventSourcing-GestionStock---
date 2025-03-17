// CommandeAddCommandHandler.cs
using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Infrastructure.Messaging;
using GestionFM1.Write.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GestionFM1.Write.Aggregates;

namespace GestionFM1.Write.CommandHandlers
{
    public class CommandeAddCommandHandler : ICommandHandler<CommandeAddCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly RabbitMqEventBus _eventBus;
        private readonly ILogger<CommandeAddCommandHandler> _logger;

        public CommandeAddCommandHandler(
            IEventStore eventStore,
            RabbitMqEventBus eventBus,
            ILogger<CommandeAddCommandHandler> logger)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Handle(CommandeAddCommand command)
        {
            try
            {
                //  l'Aggregate crée l'événement
                var aggregate = new CommandeAggregate();
                aggregate.AddCommande(
                    command.EtatCommande,
                    command.DateCmd,
                    command.ComposentId,
                    command.ExpertId,
                    command.RaisonDeCommande,
                    command.FM1Id
                );

                // Récupérer l'événement créé par l'Aggregate
                var @event = (CommandeCreatedEvent)aggregate.GetChanges().Single(); // On est sûr qu'il n'y a qu'un seul événement

                // Enregistrer l'événement dans l'Event Store
                await _eventStore.SaveEventAsync(@event);

                // Publier l'événement dans l'exchange dédié
                await _eventBus.PublishEventAsync(@event, "gestionfm1.commande.events");

                _logger.LogInformation($"Commande créée et événement publié via RabbitMQ.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la création de la commande.");
                throw;
            }
        }
    }
}