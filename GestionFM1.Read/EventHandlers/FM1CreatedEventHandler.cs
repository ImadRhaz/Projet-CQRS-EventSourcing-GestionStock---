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
    public class FM1CreatedEventHandler : IEventHandler<FM1CreatedEvent>
    {
        private readonly QueryDbContext _queryDbContext;
        private readonly ILogger<FM1CreatedEventHandler> _logger;

        public FM1CreatedEventHandler(QueryDbContext queryDbContext, ILogger<FM1CreatedEventHandler> logger)
        {
            _queryDbContext = queryDbContext;
            _logger = logger;
        }

        public async Task Handle(FM1CreatedEvent @event)
        {
            _logger.LogInformation($"Début du traitement de l'événement FM1CreatedEvent pour l'ID : {@event.FM1Id}");

            try
            {
                _logger.LogDebug($"Données de l'événement : CodeSite={@event.CodeSite}, DeviceType={@event.DeviceType}, PsSn={@event.PsSn}, DateEntre={@event.DateEntre}, ExpirationVerification={@event.ExpirationVerification}, Status={@event.Status}, ExpertId={@event.ExpertId}");

                var fm1 = new FM1
                {
                    Id = @event.FM1Id,
                    CodeSite = @event.CodeSite,
                    DeviceType = @event.DeviceType,
                    PsSn = @event.PsSn,
                    DateEntre = @event.DateEntre,
                    ExpirationVerification = @event.ExpirationVerification,
                    Status = @event.Status,
                    ExpertId = @event.ExpertId
                };

                _logger.LogDebug($"Objet FM1 créé : {System.Text.Json.JsonSerializer.Serialize(fm1)}");

                _queryDbContext.FM1s.Add(fm1);
                _logger.LogDebug("FM1 ajouté au contexte de la base de données.");

                await _queryDbContext.SaveChangesAsync();
                _logger.LogInformation($"FM1 ajouté à la base de données de lecture avec l'ID : {@event.FM1Id}");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour de la base de données pour l'ID : {@event.FM1Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur inattendue lors du traitement de l'événement FM1CreatedEvent pour l'ID : {@event.FM1Id}");
            }

            _logger.LogInformation($"Fin du traitement de l'événement FM1CreatedEvent pour l'ID : {@event.FM1Id}");
        }
    }
}