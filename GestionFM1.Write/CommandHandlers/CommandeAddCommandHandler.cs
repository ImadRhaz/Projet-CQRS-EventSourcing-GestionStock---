using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Infrastructure.Messaging;
using GestionFM1.Write.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
                var commandeId = new Random().Next(1, 1000);
                var commandeCreatedEvent = new CommandeCreatedEvent
                {
                    CommandeId = commandeId,
                    EtatCommande = command.EtatCommande,
                    DateCmd = command.DateCmd,
                    ComposentId = command.ComposentId,
                    ExpertId = command.ExpertId,
                    RaisonDeCommande = command.RaisonDeCommande,
                    FM1Id = command.FM1Id
                };

                await _eventStore.SaveEventAsync(commandeCreatedEvent);
                await _eventBus.PublishEventAsync(commandeCreatedEvent, "gestionfm1.commande.events");
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