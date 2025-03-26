using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionFM1.Infrastructure.Notification
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly RabbitMqConfiguration _config;
        private IModel _channel;
        private const string RoutingKey = "notifications.routing.key";

        public NotificationWorker(
            IConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationWorker> logger,
            IOptions<RabbitMqConfiguration> config)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[START] Initialisation du worker...");

            try
            {
                _channel = _connection.CreateModel();
                _logger.LogInformation("[RabbitMQ] Channel created successfully in StartAsync."); // LOG AJOUTÉ
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error creating channel in StartAsync: {Message}", ex.Message); // LOG AJOUTÉ
            }

            ConfigureRabbitMQChannel();
            await base.StartAsync(cancellationToken);
        }

        private void ConfigureRabbitMQChannel()
        {
            _logger.LogInformation("[ConfigureRabbitMQChannel] Configuring RabbitMQ Channel...");

            try
            {
                _channel.ExchangeDeclare(
                    exchange: _config.NotificationExchangeName,
                    type: ExchangeType.Direct,
                    durable: true);

                _channel.QueueDeclare(
                    queue: _config.NotificationQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", $"{_config.NotificationExchangeName}.dlx" },
                        { "x-message-ttl", 86400000 }
                    });

                _channel.QueueBind(
                    queue: _config.NotificationQueueName,
                    exchange: _config.NotificationExchangeName,
                    routingKey: RoutingKey);

                _logger.LogInformation("[BINDING] Queue bindée avec la clé: {Key}", RoutingKey);
                _logger.LogInformation("[ConfigureRabbitMQChannel] RabbitMQ Channel configured successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ConfigureRabbitMQChannel] Error configuring RabbitMQ Channel: {Message}", ex.Message);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[EXECUTE] ExecuteAsync démarrée");

            StartConsuming(); // Use BasicConsume
            _logger.LogInformation("[EXECUTE] Started consuming messages.  Waiting for messages...");

            await Task.CompletedTask;  // Prevent the method from completing immediately.
        }

        private void StartConsuming()
        {
            _logger.LogInformation("[StartConsuming] Starting message consumer...");
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    ulong deliveryTag = ea.DeliveryTag;

                    _logger.LogInformation($"[CONSUME] Message reçu directement : {message}");

                    try
                    {
                        var notification = JsonConvert.DeserializeObject<NotificationEvent>(message);

                        using var scope = _scopeFactory.CreateScope();
                        var storage = scope.ServiceProvider.GetRequiredService<INotificationStorageService>();

                        _logger.LogInformation($"[SAUVEGARDE] Tentative d'enregistrement pour notification {notification.Id} et le message: {notification.Message}");

                        await storage.SaveNotificationAsync(notification);

                        _logger.LogInformation($"[SUCCES] Notification {notification.Id} enregistrée en base");

                        _channel.BasicAck(deliveryTag, false);
                        _logger.LogDebug($"[ACK] Message with delivery tag {deliveryTag} acknowledged.");
                    }
                    catch (JsonSerializationException jex)
                    {
                        _logger.LogError(jex, "[CONSUME] Erreur lors de la désérialisation du message: {Message}", jex.Message);
                        _channel.BasicNack(deliveryTag, false, false); // Reject, don't requeue
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[CONSUME] Erreur lors du traitement du message: {Message}", ex.Message);
                        _channel.BasicNack(deliveryTag, false, false); // Reject, don't requeue
                    }
                };

                _channel.BasicConsume(queue: _config.NotificationQueueName, autoAck: false, consumer: consumer);
                _logger.LogInformation($"[BasicConsume] Consuming from queue: {_config.NotificationQueueName} with autoAck=false");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StartConsuming] Error starting consumer: {Message}", ex.Message);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[StopAsync] Stopping the worker...");
            try
            {
                _channel?.Close();
                _logger.LogInformation("[StopAsync] Channel closed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StopAsync] Error closing channel: {Message}", ex.Message);
            }
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("[StopAsync] Worker stopped successfully.");

        }

        public override void Dispose()
        {
            _logger.LogInformation("[Dispose] Disposing of resources...");
            try
            {
                _channel?.Dispose();
                _logger.LogInformation("[Dispose] Channel disposed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Dispose] Error disposing channel: {Message}", ex.Message);
            }
            base.Dispose();
            _logger.LogInformation("[Dispose] Resources disposed successfully.");
        }
    }
}