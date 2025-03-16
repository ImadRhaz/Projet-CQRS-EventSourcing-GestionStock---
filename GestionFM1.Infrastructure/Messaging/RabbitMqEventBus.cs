using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GestionFM1.Infrastructure.Messaging;

public class RabbitMqEventBus
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<string, List<Func<string, Task>>> _eventHandlers = new();

    public RabbitMqEventBus(IOptions<RabbitMqConfiguration> rabbitMqConfiguration, ILogger<RabbitMqEventBus> logger)
    {
        _rabbitMqConfiguration = rabbitMqConfiguration.Value;
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfiguration.HostName,
            UserName = _rabbitMqConfiguration.UserName,
            Password = _rabbitMqConfiguration.Password
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("RabbitMQ connection and channel created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating RabbitMQ connection or channel.");
            throw;
        }
    }

    public async Task PublishEventAsync(object @event, string exchangeName)
    {
        try
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);

            var json = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);

            _logger.LogInformation($"Event '{@event.GetType().Name}' published to exchange '{exchangeName}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing event to exchange '{exchangeName}'.");
            throw;
        }
    }

    public void Subscribe<TEvent>(string exchangeName, Func<string, Task> handler)
    {
        var eventName = typeof(TEvent).Name;

        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName] = new List<Func<string, Task>>();
        }

        _eventHandlers[eventName].Add(handler);

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await handler(message);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing event from queue '{queueName}'.");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation($"Subscribed to event '{eventName}' on exchange '{exchangeName}'.");
    }
}