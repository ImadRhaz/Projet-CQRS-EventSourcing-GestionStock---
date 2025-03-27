using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Infrastructure.Messaging;
using GestionFM1.Write.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GestionFM1.Write.CommandHandlers
{
    public class AddFM1HistoryCommandHandler : ICommandHandler<AddFM1HistoryCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly RabbitMqEventBus _eventBus;
        private readonly ILogger<AddFM1HistoryCommandHandler> _logger;

        public AddFM1HistoryCommandHandler(
            IEventStore eventStore,
            RabbitMqEventBus eventBus,
            ILogger<AddFM1HistoryCommandHandler> logger)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task Handle(AddFM1HistoryCommand command)
        {
            try
            {
                var fm1HistoryId = Guid.NewGuid();

                var fm1HistoryCreatedEvent = new FM1HistoryCreatedEvent
                {
                    FM1HistoryId = fm1HistoryId,
                    FM1Id = command.FM1Id
                };

                await _eventStore.SaveEventAsync(fm1HistoryCreatedEvent);
                await _eventBus.PublishEventAsync(fm1HistoryCreatedEvent, "gestionfm1.fm1history.events");

                _logger.LogInformation($"FM1History créé avec l'ID : {fm1HistoryId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du FM1History.");
                throw;
            }
        }
    }
}