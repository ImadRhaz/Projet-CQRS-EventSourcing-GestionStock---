using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GestionFM1.Infrastructure.Notification;
using Microsoft.AspNetCore.SignalR;

namespace GestionFM1.Infrastructure.Messaging
{
    public class CommandeValidatedWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CommandeValidatedWorker> _logger;
        private readonly RabbitMqConfiguration _config;
        private IModel _channel;

        public CommandeValidatedWorker(
            IConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<CommandeValidatedWorker> logger,
            IOptions<RabbitMqConfiguration> config)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[START] Initialisation du worker de commandes validées...");

            try
            {
                _channel = _connection.CreateModel();
                ConfigureRabbitMQChannel();
                _logger.LogInformation("[RabbitMQ] Channel créé avec succès pour les commandes validées");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Erreur lors de la création du channel");
                throw;
            }

            await base.StartAsync(cancellationToken);
        }

        private void ConfigureRabbitMQChannel()
        {
            try
            {
                _channel.QueueDeclare(
                    queue: _config.CommandeValidatedQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", $"{_config.CommandeValidatedQueue}.dlx" }
                    });

                _logger.LogInformation("[CONFIG] Queue {Queue} configurée", _config.CommandeValidatedQueue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CONFIG] Erreur de configuration RabbitMQ");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[EXECUTE] Démarrage de la consommation des messages...");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deliveryTag = ea.DeliveryTag;

                try
                {
                    _logger.LogInformation("[TRAITEMENT] Réception d'une commande validée: {Message}", message);

                    var commande = JsonConvert.DeserializeObject<CommandeValidatedMessage>(message);
                    
                    using var scope = _scopeFactory.CreateScope();
                    var storageService = scope.ServiceProvider.GetRequiredService<INotificationStorageService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    // Création de la notification avec NotificationEvent directement
                    var notification = new NotificationEvent
                    {
                        Id = Guid.NewGuid(),
                        UserId = commande.ExpertId,
                        Title = "Commande Validée",
                        Message = $"La commande #{commande.CommandeId} a été validée",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        CommandeId = commande.CommandeId,
                        NotificationType = "CommandeValidated"
                    };

                    // Sauvegarde en base
                    await storageService.SaveNotificationAsync(notification);
                    
                    // Notification en temps réel via SignalR
                    await hubContext.Clients.Group(commande.ExpertId)
                        .SendAsync("ReceiveCommandeValidated", notification);

                    _channel.BasicAck(deliveryTag, false);
                    _logger.LogInformation("[SUCCÈS] Commande {CommandeId} traitée", commande.CommandeId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ERREUR] Traitement de la commande");
                    _channel.BasicNack(deliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: _config.CommandeValidatedQueue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("[ECOUTE] En attente de commandes validées...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[ARRÊT] Fermeture du worker...");
            try
            {
                _channel?.Close();
                _logger.LogInformation("[ARRÊT] Channel fermé avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ARRÊT] Erreur lors de la fermeture");
            }
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            base.Dispose();
        }
    }

    public class CommandeValidatedMessage
    {
        public int CommandeId { get; set; }
        public string ExpertId { get; set; }
        public DateTime ValidationDate { get; set; } = DateTime.UtcNow;
    }
}