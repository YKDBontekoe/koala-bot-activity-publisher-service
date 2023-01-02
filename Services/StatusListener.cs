using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Constants;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Interfaces;

namespace Koala.ActivityPublisherService.Services;

public class StatusListener : IStatusListener
{
    private readonly DiscordSocketClient _client;
    private readonly IMessageService _messageService;

    public StatusListener(DiscordSocketClient client, IMessageService messageService)
    {
        _client = client;
        _messageService = messageService;
    }

    public Task InitializeAsync()
    {
        _client.PresenceUpdated += SendStatusUpdates;
        
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _client.DisposeAsync();
    }
    
    // This method is called whenever a user's status changes
    private async Task SendStatusUpdates(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user.IsBot)
        {
            return;
        }

        await SendStatusUpdate(user, newPresence.Status.ToString());
    }
    
    // This method sends the status update via the message service
    private async Task SendStatusUpdate(IUser user, string statusDetails)
    {
        await _messageService.SendMessage(new Activity
        {
            User = new User
            {
                Id = user.Id,
                Username = user.Username,
            },
            StartedAt = DateTimeOffset.UtcNow,
            Type = MessageTypes.Status,
            Details = statusDetails,
            Name = statusDetails,
        });
    }
}