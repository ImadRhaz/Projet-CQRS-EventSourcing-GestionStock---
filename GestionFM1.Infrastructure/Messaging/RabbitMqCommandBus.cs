using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Infrastructure.Messaging;

public class RabbitMqCommandBus
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    private readonly ILogger<RabbitMqCommandBus> _logger;

    public RabbitMqCommandBus(IOptions<RabbitMqConfiguration> rabbitMqConfiguration, ILogger<RabbitMqCommandBus> logger)
    {
        _rabbitMqConfiguration = rabbitMqConfiguration.Value;
        _logger = logger;
    }

    public async Task SendCommandAsync(object command, string queueName)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfiguration.HostName,
            UserName = _rabbitMqConfiguration.UserName,
            Password = _rabbitMqConfiguration.Password
        };

        try
        {
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonConvert.SerializeObject(command);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
            _logger.LogInformation($"Command '{command.GetType().Name}' sent to queue '{queueName}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending command to queue '{queueName}'.");
            throw;
        }
    }
}