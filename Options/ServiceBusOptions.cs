namespace Koala.ActivityPublisherService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string UserMusicQueueName { get; set; } = string.Empty;
    public string UserGameQueueName { get; set; } = string.Empty;
    public string ActivitiesConsumerQueueName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}