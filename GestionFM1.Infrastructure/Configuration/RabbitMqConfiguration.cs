namespace GestionFM1.Infrastructure.Configuration;

public class RabbitMqConfiguration
{
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NotificationQueueName { get; set; } = "notifications.queue";
    public string NotificationExchangeName { get; set; } = "notifications.exchange";
    public string CommandeValidatedQueue { get; set; } = "commandes.validees.queue";

        public int Port { get; set; } = 5672;
    
}