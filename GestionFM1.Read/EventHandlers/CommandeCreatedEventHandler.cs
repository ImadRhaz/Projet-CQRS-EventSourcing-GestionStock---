using GestionFM1.Core.Events;
using GestionFM1.Core.Interfaces;
using GestionFM1.Read.QueryDataStore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GestionFM1.Core.Models;
using GestionFM1.Infrastructure.Notification;
using Newtonsoft.Json;

namespace GestionFM1.Read.EventHandlers
{
    public class CommandeCreatedEventHandler : IEventHandler<CommandeCreatedEvent>
    {
        private readonly QueryDbContext _queryDbContext;
        private readonly ILogger<CommandeCreatedEventHandler> _logger;
        private readonly INotificationQueueService _notificationQueue;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CommandeCreatedEventHandler(
            QueryDbContext queryDbContext,
            ILogger<CommandeCreatedEventHandler> logger,
            INotificationQueueService notificationQueue,
            IHubContext<NotificationHub> hubContext)
        {
            _queryDbContext = queryDbContext;
            _logger = logger;
            _notificationQueue = notificationQueue;
            _hubContext = hubContext;
        }

        public async Task Handle(CommandeCreatedEvent @event)
        {
            _logger.LogInformation($"Début du traitement de CommandeCreatedEvent pour le composant : {@event.ComposentId}");

            try
            {
                // Créer la commande sans spécifier l'Id
                var commande = new Commande
                {
                    EtatCommande = @event.EtatCommande,
                    DateCmd = @event.DateCmd,
                    ComposentId = @event.ComposentId,
                    ExpertId = @event.ExpertId,
                    RaisonDeCommande = @event.RaisonDeCommande,
                    FM1Id = @event.FM1Id,
                    FM1HistoryId = @event.FM1HistoryId
                };

                _queryDbContext.Commandes.Add(commande);
                await _queryDbContext.SaveChangesAsync();

                // Log après l'enregistrement de la commande
                _logger.LogInformation($"Commande enregistrée avec succès. ID: {commande.Id}");


                // Mettre à jour le composant avec le nouvel ID généré
                var composent = await _queryDbContext.Composents.FindAsync(@event.ComposentId);
                if (composent != null)
                {
                    composent.OrderOrNot = "Commandé";
                    composent.CommandeId = commande.Id; // Utilisation de l'ID auto-généré
                    await _queryDbContext.SaveChangesAsync();

                     _logger.LogInformation($"Composent mit à jour avec succès. ID: {composent.Id}");
                }

                // Créer l'événement de notification avec le nouvel ID
                  _logger.LogInformation($"Avant la création de la notification : commande.Id = {commande.Id}, ExpertId = {@event.ExpertId}");
                var notification = new NotificationEvent
                {
                    Title = "Nouvelle Commande",
                    Message = $"Une nouvelle commande a été créée (ID: {commande.Id})",
                    CommandeId = commande.Id,
                    UserId = @event.ExpertId
                };

                      _logger.LogInformation($"Après la création de la notification: NotificationId={notification.Id},  CommandeId={notification.CommandeId}, UserId={notification.UserId}");
                      // Log AJOUTÉ - Avant d'ajouter a la queue on LOG le message en entier et on regarde les erreurs
                      _logger.LogInformation("[AVANT QUEUE] Notification à ajouter : {Notification}", JsonConvert.SerializeObject(notification));

                      // Envoi à la file d'attente (persistance)
                         _logger.LogInformation($"Avant EnqueueNotificationAsync : NotificationId={notification.Id}");
                await _notificationQueue.EnqueueNotificationAsync(notification);
                         _logger.LogInformation($"Après EnqueueNotificationAsync : NotificationId={notification.Id}");

                // Tentative d'envoi en temps réel
                try
                {
                      _logger.LogInformation($"Avant HubContext en temps reel ");
                    await _hubContext.Clients.User(@event.ExpertId)
                        .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Apres HubContext en temps reel ");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Échec de la notification en temps réel - stockée dans la file d'attente");
                }

                _logger.LogInformation($"Commande {commande.Id} ajoutée et notification envoyée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de l'événement CommandeCreatedEvent.");
                throw;
            }
        }
    }
}