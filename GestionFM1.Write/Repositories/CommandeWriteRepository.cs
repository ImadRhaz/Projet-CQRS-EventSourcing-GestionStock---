using GestionFM1.Write.Commands;
using GestionFM1.Write.Aggregates;
using GestionFM1.Write.EventStore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.Repositories
{
    public class CommandeWriteRepository
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger<CommandeWriteRepository> _logger;

        public CommandeWriteRepository(IEventStore eventStore, ILogger<CommandeWriteRepository> logger)
        {
            _eventStore = eventStore;
            _logger = logger;
        }

        public async Task AddCommandeAsync(CommandeAddCommand command)
        {
            try
            {
                var aggregate = new CommandeAggregate();
                aggregate.AddCommande(
                    command.EtatCommande,
                    command.DateCmd,
                    command.ComposentId,
                    command.ExpertId,
                    command.RaisonDeCommande,
                    command.FM1Id
                );

                foreach (var @event in aggregate.GetChanges())
                {
                    await _eventStore.SaveEventAsync(@event);
                }
                _logger.LogInformation($"Commande ajoutée à l'EventStore");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout de la commande à l'EventStore");
                throw;
            }
        }
    }
}