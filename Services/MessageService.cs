using Azure.Messaging.ServiceBus;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Options;
using Koala.ActivityPublisherService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityPublisherService.Services;

public class MessageService : IMessageService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    
    public MessageService(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions)
    {
        _serviceBusClient = serviceBusClient;
        _serviceBusOptions = serviceBusOptions != null ? serviceBusOptions.Value : throw new ArgumentNullException(nameof(serviceBusOptions));
    }
    
    // Sends a message to the queue
    public async Task SendMessage(Activity activity)
    {
        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.UserActivitiesQueueName);
        var message = new ServiceBusMessage(JsonConvert.SerializeObject(activity));
        
        await sender.SendMessageAsync(message);
    }
}