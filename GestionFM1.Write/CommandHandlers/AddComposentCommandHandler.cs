using GestionFM1.Write.Commands;
using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GestionFM1.Write.CommandHandlers
{
    public class AddComposentCommandHandler : ICommandHandler<AddComposentCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly RabbitMqEventBus _eventBus;
        private readonly ILogger<AddComposentCommandHandler> _logger;

        public AddComposentCommandHandler(
            IEventStore eventStore,
            RabbitMqEventBus eventBus,
            ILogger<AddComposentCommandHandler> logger)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Handle(AddComposentCommand command)
        {
            try
            {
                var composentId = Guid.NewGuid(); // Générer un nouvel ID Guid

                var composentCreatedEvent = new ComposentCreatedEvent
                {
                    ComposentId = composentId,
                    ItemBaseId = command.ItemBaseId,
                    ProductName = command.ProductName,
                    SN = command.SN,
                    TotalAvailable = command.TotalAvailable,
                    UrgentOrNot = command.UrgentOrNot,
                    OrderOrNot = command.OrderOrNot,
                    FM1Id = command.FM1Id
                };

                // Enregistrer l'événement dans l'Event Store
                await _eventStore.SaveEventAsync(composentCreatedEvent);

                // Publier l'événement dans l'exchange dédié
                await _eventBus.PublishEventAsync(composentCreatedEvent, "gestionfm1.composent.events");

                _logger.LogInformation($"Composent créé avec l'ID : {composentId}");
                _logger.LogInformation($"Événement publié via RabbitMQ pour le Composent : {composentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la création du Composent.");
                throw;
            }
        }
    }
}