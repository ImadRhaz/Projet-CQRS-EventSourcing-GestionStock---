using GestionFM1.Core.Events;
using GestionFM1.Read.QueryDataStore;
using System.Threading.Tasks;
using GestionFM1.Core.Models;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace GestionFM1.Read.EventHandlers
{
    public class ComposentCreatedEventHandler : IEventHandler<ComposentCreatedEvent>
    {
        private readonly QueryDbContext _queryDbContext;
        private readonly ILogger<ComposentCreatedEventHandler> _logger;

        public ComposentCreatedEventHandler(QueryDbContext queryDbContext, ILogger<ComposentCreatedEventHandler> logger)
        {
            _queryDbContext = queryDbContext;
            _logger = logger;

            _logger.LogInformation("✅ ComposentCreatedEventHandler initialisé et prêt à écouter les événements.");
        }

        public async Task Handle(ComposentCreatedEvent @event)
        {
            _logger.LogInformation($"🔄 Début du traitement de ComposentCreatedEvent pour l'ID : {@event.ComposentId}");

            bool canConnect = _queryDbContext.Database.CanConnect();
            _logger.LogDebug($"🔍 Vérification de la connexion à la base de lecture : {canConnect}");

            if (!canConnect)
            {
                _logger.LogError("❌ Impossible de se connecter à la base de lecture !");
                return;
            }

            try
            {
                _logger.LogDebug($"Données de l'événement : ItemBaseId={@event.ItemBaseId}, ProductName={@event.ProductName}, SN={@event.SN}, TotalAvailable={@event.TotalAvailable}, UrgentOrNot={@event.UrgentOrNot}, OrderOrNot={@event.OrderOrNot}, FM1Id={@event.FM1Id}");

                var composent = new Composent
                {
                    Id = @event.ComposentId,
                    ItemBaseId = @event.ItemBaseId,
                    ProductName = @event.ProductName,
                    SN = @event.SN,
                    TotalAvailable = @event.TotalAvailable,
                    UrgentOrNot = @event.UrgentOrNot,
                    OrderOrNot = @event.OrderOrNot,
                    FM1Id = @event.FM1Id
                };

                _logger.LogDebug($"📌 Objet Composent créé : {System.Text.Json.JsonSerializer.Serialize(composent)}");

                _logger.LogDebug("📌 Avant d'ajouter Composent au contexte de la base de données.");
                _queryDbContext.Set<Composent>().Add(composent); // Use Set<Composent>()
                _logger.LogDebug("📌 Après avoir ajouté Composent au contexte de la base de données.");

                _logger.LogDebug("💾 Avant d'enregistrer les modifications dans la base de données.");
                await _queryDbContext.SaveChangesAsync();
                _logger.LogDebug("✅ Après avoir enregistré les modifications dans la base de données.");

                _logger.LogInformation($"✅ Composent ajouté à la base de données de lecture avec l'ID : {@event.ComposentId}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"❌ Erreur lors de la mise à jour de la base de données pour l'ID : {@event.ComposentId}");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "❌ Inner Exception de DbUpdateException");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Erreur inattendue lors du traitement de l'événement ComposentCreatedEvent pour l'ID : {@event.ComposentId}");
            }

            _logger.LogInformation($"🏁 Fin du traitement de l'événement ComposentCreatedEvent pour l'ID : {@event.ComposentId}");
        }
    }
}