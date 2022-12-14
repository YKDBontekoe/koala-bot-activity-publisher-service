using System.Reflection;
using Azure.Messaging.ServiceBus;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
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
    public async Task SendMessage(Activity activity, Type? type)
    {
        type ??= activity.GetType();
        var sender = _serviceBusClient.CreateSender(GetQueueName(type));
        var serializedActivity = JsonConvert.SerializeObject(Convert.ChangeType(activity, type));
        var message = new ServiceBusMessage(serializedActivity);
        
        await sender.SendMessageAsync(message);
    }
    
    private string GetQueueName(MemberInfo type)
    {
        return type.Name switch
        {
            nameof(GameActivity) => _serviceBusOptions.UserGameQueueName,
            nameof(SpotifyActivity) => _serviceBusOptions.UserMusicQueueName,
            _ => _serviceBusOptions.ActivitiesConsumerQueueName
        };
    }
}