namespace Koala.ActivityPublisherService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string UserActivitiesQueueName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}