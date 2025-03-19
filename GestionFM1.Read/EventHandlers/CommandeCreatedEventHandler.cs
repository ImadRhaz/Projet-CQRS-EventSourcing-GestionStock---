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
            _logger.LogInformation($"Début du traitement de CommandeCreatedEvent pour l'ID : {@event.CommandeId}");

            try
            {
                // Créer la commande
                var commande = new Commande
                {
                    EtatCommande = @event.EtatCommande,
                    DateCmd = @event.DateCmd,
                    ComposentId = @event.ComposentId,
                    ExpertId = @event.ExpertId,
                    RaisonDeCommande = @event.RaisonDeCommande,
                    FM1Id = @event.FM1Id,
                    FM1HistoryId = @event.FM1HistoryId // Utiliser la valeur de l'événement
                };

                _queryDbContext.Commandes.Add(commande);
                await _queryDbContext.SaveChangesAsync();

                  //Get the component and update it, and the commande id.
                var composent = await _queryDbContext.Composents.FindAsync(@event.ComposentId);
                if (composent != null)
                {
                    composent.OrderOrNot = "Commandé";
                     composent.CommandeId = commande.Id;  // <--- Mettre à jour CommandeId du composant
                    await _queryDbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"Commande ajoutée à la base de données de lecture, associée à FM1HistoryId : {@event.FM1HistoryId}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la base de données.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de l'événement CommandeCreatedEvent.");
            }

            _logger.LogInformation($"Fin du traitement de l'événement CommandeCreatedEvent");
        }
    }
}