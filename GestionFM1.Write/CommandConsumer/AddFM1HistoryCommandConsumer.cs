using GestionFM1.Infrastructure.Configuration;
using GestionFM1.Write.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Write.CommandConsumer
{
    public class AddFM1HistoryCommandConsumer : IHostedService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "gestionfm1.fm1history.commands";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AddFM1HistoryCommandConsumer> _logger;
        private bool _disposed = false;

        public AddFM1HistoryCommandConsumer(
            IOptions<RabbitMqConfiguration> rabbitMqConfiguration,
            IServiceProvider serviceProvider,
            ILogger<AddFM1HistoryCommandConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqConfiguration.Value.HostName,
                UserName = rabbitMqConfiguration.Value.UserName,
                Password = rabbitMqConfiguration.Value.Password
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _logger.LogInformation("RabbitMQ connection and channel created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RabbitMQ connection or channel.");
                throw;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddFM1HistoryCommandConsumer starting");
            Consume(); // Start consuming messages
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddFM1HistoryCommandConsumer stopping");
            Dispose(); // Clean up resources
            return Task.CompletedTask;
        }

        private void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    _logger.LogInformation($"Received message: {message}");
                    var command = JsonConvert.DeserializeObject<AddFM1HistoryCommand>(message);

                    if (command == null)
                    {
                        _logger.LogError("Failed to deserialize message to AddFM1HistoryCommand.");
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        return;
                    }

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var addFM1HistoryCommandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddFM1HistoryCommand>>();

                        if (addFM1HistoryCommandHandler == null)
                        {
                            _logger.LogError("Failed to resolve ICommandHandler<AddFM1HistoryCommand> from DI container.");
                            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                            return;
                        }

                        await addFM1HistoryCommandHandler.Handle(command);
                    }

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogInformation($"Successfully processed message.");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, $"JSON deserialization error for message: {message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message: {message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Listening for messages...");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                try
                {
                    if (_channel != null && _channel.IsOpen)
                    {
                        _channel.Close();
                        _logger.LogInformation("RabbitMQ channel closed.");
                    }
                    if (_connection != null && _connection.IsOpen)
                    {
                        _connection.Close();
                        _logger.LogInformation("RabbitMQ connection closed.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing RabbitMQ connection or channel.");
                }
            }
            _disposed = true;
        }
    }
}