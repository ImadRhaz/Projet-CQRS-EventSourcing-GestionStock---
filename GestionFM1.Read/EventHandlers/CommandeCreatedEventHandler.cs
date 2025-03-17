using GestionFM1.Core.Events;
using GestionFM1.Read.QueryDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Models;
using GestionFM1.Core.Interfaces;

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

            _logger.LogInformation("✅ CommandeCreatedEventHandler initialisé et prêt à écouter les événements.");
        }

        public async Task Handle(CommandeCreatedEvent @event)
        {
            _logger.LogInformation($"🔄 Début du traitement de CommandeCreatedEvent pour l'ID : {@event.CommandeId}");

            bool canConnect = _queryDbContext.Database.CanConnect();
            _logger.LogDebug($"🔍 Vérification de la connexion à la base de lecture : {canConnect}");

            if (!canConnect)
            {
                _logger.LogError("❌ Impossible de se connecter à la base de lecture !");
                return;
            }

            try
            {
                _logger.LogDebug($"Données de l'événement : EtatCommande={@event.EtatCommande}, DateCmd={@event.DateCmd}, ComposentId={@event.ComposentId}, ExpertId={@event.ExpertId}, RaisonDeCommande={@event.RaisonDeCommande}, FM1Id={@event.FM1Id}");

                var commande = new Commande
                {
                    Id = @event.CommandeId,  // Récupérer l'ID auto-incrémenté
                    EtatCommande = @event.EtatCommande,
                    DateCmd = @event.DateCmd,
                    ComposentId = @event.ComposentId,
                    ExpertId = @event.ExpertId,
                    RaisonDeCommande = @event.RaisonDeCommande,
                    FM1Id = @event.FM1Id
                };

                _logger.LogDebug($"📌 Objet Commande créé : {System.Text.Json.JsonSerializer.Serialize(commande)}");

                _logger.LogDebug("📌 Avant d'ajouter Commande au contexte de la base de données.");
                _queryDbContext.Set<Commande>().Add(commande); // Use Set<Commande>()
                _logger.LogDebug("📌 Après avoir ajouté Commande au contexte de la base de données.");

                _logger.LogDebug("💾 Avant d'enregistrer les modifications dans la base de données.");
                await _queryDbContext.SaveChangesAsync();
                _logger.LogDebug("✅ Après avoir enregistré les modifications dans la base de données.");

                // Now, retrieve the auto-incremented ID
                @event.CommandeId = commande.Id; // update the event with the id
                _logger.LogInformation($"✅ Commande ajouté à la base de données de lecture avec l'ID : {@event.CommandeId}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"❌ Erreur lors de la mise à jour de la base de données pour l'ID : {@event.CommandeId}");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "❌ Inner Exception de DbUpdateException");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Erreur inattendue lors du traitement de l'événement CommandeCreatedEvent pour l'ID : {@event.CommandeId}");
            }

            _logger.LogInformation($"🏁 Fin du traitement de l'événement CommandeCreatedEvent pour l'ID : {@event.CommandeId}");
        }
    }
}