using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GestionFM1.Infrastructure.Notification
{
   
public class NotificationQueueService : INotificationQueueService
    {
        private readonly IModel _channel;
        private readonly RabbitMqConfiguration _config;
        private readonly ILogger<NotificationQueueService> _logger;
        private const string RoutingKey = "notifications.routing.key";

        public NotificationQueueService(
            IConnection connection,
            IOptions<RabbitMqConfiguration> config,
            ILogger<NotificationQueueService> logger)
        {
            _config = config.Value;
            _logger = logger;
            _channel = connection.CreateModel();
            ConfigureRabbitMQ();
        }

        private void ConfigureRabbitMQ()
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

            _logger.LogInformation("[CONFIG] Service configuré pour Exchange: {Exchange}, Queue: {Queue}, RoutingKey: {Key}",
                _config.NotificationExchangeName,
                _config.NotificationQueueName,
                RoutingKey);
        }

       
public Task EnqueueNotificationAsync(NotificationEvent notification)
{
    try
    {
        // Vérification que le canal est valide
        if (_channel.IsClosed)
        {
            _logger.LogError("[ERREUR] Canal RabbitMQ fermé");
            throw new InvalidOperationException("Canal RabbitMQ non disponible");
        }

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notification));
        
        _channel.BasicPublish(
            exchange: _config.NotificationExchangeName,
            routingKey: RoutingKey,
            mandatory: true, // Important: génère une exception si le message ne peut être routé
            basicProperties: properties,
            body: body);
        
        _logger.LogInformation("[PUBLICATION REUSSIE] Notification {Id} routée vers {Exchange} avec clé {Key}",
            notification.Id,
            _config.NotificationExchangeName,
            RoutingKey);
            
        return Task.CompletedTask;
    }
    catch (Exception ex)
    {
        _logger.LogCritical(ex, "[ERREUR PUBLICATION] Échec critique de la publication");
        throw;
    }
}

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}