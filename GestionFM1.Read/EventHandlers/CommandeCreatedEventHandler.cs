using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Read.QueryDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Models;

namespace GestionFM1.Read.EventHandlers
{
    public class CommandeCreatedEventHandler : IEventHandler<CommandeCreatedEvent>
    {
        private readonly QueryDbContext _queryDbContext;
        private readonly ILogger<CommandeCreatedEventHandler> _logger;

        public CommandeCreatedEventHandler(QueryDbContext queryDbContext, ILogger<CommandeCreatedEventHandler> logger)
        {
            _queryDbContext = queryDbContext;
            _logger = logger;
        }

        public async Task Handle(CommandeCreatedEvent @event)
        {
            _logger.LogInformation($"🔄 Début du traitement de CommandeCreatedEvent pour l'ID : {@event.CommandeId}");

            try
            {
                Guid? fm1HistoryId = null;

                // Vérifier si un FM1History existe déjà pour cet FM1
                var existingFM1History = await _queryDbContext.FM1Histories
                    .FirstOrDefaultAsync(vh => vh.FM1Id == @event.FM1Id);

                if (existingFM1History == null)
                {
                    _logger.LogInformation($"Création d'un nouveau FM1History pour FM1Id : {@event.FM1Id}");

                    // Créer un nouvel FM1History
                    var newFM1History = new FM1History
                    {
                        Id = Guid.NewGuid(),
                        FM1Id = @event.FM1Id
                    };

                    _queryDbContext.FM1Histories.Add(newFM1History);
                    await _queryDbContext.SaveChangesAsync();

                    fm1HistoryId = newFM1History.Id; // Récupérer l'ID du nouvel FM1History
                }
                else
                {
                    fm1HistoryId = existingFM1History.Id; // Utiliser l'ID de l'FM1History existant
                }

                var commande = new Commande
                {
                    Id = @event.CommandeId,
                    EtatCommande = @event.EtatCommande,
                    DateCmd = @event.DateCmd,
                    ComposentId = @event.ComposentId,
                    ExpertId = @event.ExpertId,
                    RaisonDeCommande = @event.RaisonDeCommande,
                    FM1Id = @event.FM1Id,
                    FM1HistoryId = fm1HistoryId // Associer la commande à l'historique
                };

                _queryDbContext.Commandes.Add(commande);
                await _queryDbContext.SaveChangesAsync();

                _logger.LogInformation($"Commande ajoutée à la base de données de lecture avec l'ID : {@event.CommandeId}, associée à FM1HistoryId : {fm1HistoryId}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la base de données.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de l'événement CommandeCreatedEvent.");
            }

            _logger.LogInformation($"🏁 Fin du traitement de l'événement CommandeCreatedEvent pour l'ID : {@event.CommandeId}");
        }
    }
}