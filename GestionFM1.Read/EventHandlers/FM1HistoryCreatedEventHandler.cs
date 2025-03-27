using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Read.QueryDataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionFM1.Read.EventHandlers
{
    public class FM1HistoryCreatedEventHandler : IEventHandler<FM1HistoryCreatedEvent>
    {
        private readonly QueryDbContext _queryDbContext;
        private readonly ILogger<FM1HistoryCreatedEventHandler> _logger;

        public FM1HistoryCreatedEventHandler(QueryDbContext queryDbContext, ILogger<FM1HistoryCreatedEventHandler> logger)
        {
            _queryDbContext = queryDbContext;
            _logger = logger;
        }

        public async Task Handle(FM1HistoryCreatedEvent @event)
        {
            try
            {
                var fm1History = new FM1History
                {
                    Id = @event.FM1HistoryId,
                    FM1Id = @event.FM1Id
                };

                _queryDbContext.FM1Histories.Add(fm1History);
                await _queryDbContext.SaveChangesAsync();

                _logger.LogInformation($"FM1History ajouté à la base de données de lecture avec l'ID : {@event.FM1HistoryId}");

                 // Mise à jour de FM1 avec FM1HistoryId
                 var fm1 = await _queryDbContext.FM1s.FindAsync(@event.FM1Id);
                 if (fm1 != null)
                 {
                     fm1.FM1HistoryId = @event.FM1HistoryId;
                     await _queryDbContext.SaveChangesAsync();
                     _logger.LogInformation($"FM1 mis à jour avec FM1HistoryId : {@event.FM1HistoryId}");
                 }
                 else
                 {
                     _logger.LogWarning($"FM1 non trouvé avec l'ID : {@event.FM1Id}");
                 }
            }
            catch (DbUpdateException ex)
            {
                 _logger.LogError(ex, $"Erreur lors de la mise à jour de la base de données pour l'ID : {@event.FM1HistoryId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du traitement de l'événement FM1HistoryCreatedEvent pour l'ID : {@event.FM1HistoryId}");
            }
        }
    }
}