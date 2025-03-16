using GestionFM1.Write.Commands;
using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GestionFM1.Write.CommandHandlers
{
    public class AddFM1CommandHandler : ICommandHandler<AddFM1Command>
    {
        private readonly IEventStore _eventStore;
        private readonly RabbitMqEventBus _eventBus;
        private readonly ILogger<AddFM1CommandHandler> _logger;

        public AddFM1CommandHandler(
            IEventStore eventStore,
            RabbitMqEventBus eventBus,
            ILogger<AddFM1CommandHandler> logger)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Handle(AddFM1Command command)
        {
            try
            {
                var fm1Id = Guid.NewGuid();

                var fm1CreatedEvent = new FM1CreatedEvent
                {
                    FM1Id = fm1Id,
                    CodeSite = command.CodeSite,
                    DeviceType = command.DeviceType,
                    PsSn = command.PsSn,
                    DateEntre = command.DateEntre,
                    ExpirationVerification = command.ExpirationVerification,
                    Status = command.Status,
                    ExpertId = command.ExpertId
                };

                // Enregistrer l'événement dans l'Event Store
                await _eventStore.SaveEventAsync(fm1CreatedEvent);

                // Publier l'événement dans l'exchange dédié
                await _eventBus.PublishEventAsync(fm1CreatedEvent, "gestionFM1.fm1.events");

                _logger.LogInformation($"FM1 créé avec l'ID : {fm1Id}");
                _logger.LogInformation($"Événement publié via RabbitMQ pour FM1 : {fm1Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la création de FM1.");
                throw;
            }
        }
    }
}