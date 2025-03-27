namespace GestionFM1.Infrastructure.Configuration
{
    public class NotificationConfiguration
    {
        public int RetryCount { get; set; } = 3;
        public int ExpirationHours { get; set; } = 24;
    }
}