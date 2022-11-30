using Azure.Messaging.ServiceBus;
using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Options;
using Koala.ActivityPublisherService.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Koala.ActivityPublisherService.Services;

public class ActivityListener : IActivityListener
{
    private readonly DiscordSocketClient _client;
    private readonly DiscordOptions _discordOptions;
    private readonly IMessageService _messageService;

    public ActivityListener(DiscordSocketClient client, IOptions<DiscordOptions> discordOptions, IMessageService messageService)
    {
        _client = client;
        _messageService = messageService;
        _discordOptions = discordOptions != null ? discordOptions.Value : throw new ArgumentNullException(nameof(discordOptions));
    }

    // Initialize the discord message events
    public async Task InitializeAsync()
    {
        await InitializeDiscordClient();
        _client.PresenceUpdated += Client_PresenceUpdated;
    }

    private async Task Client_PresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        foreach (var activity in newPresence.Activities)
        {
            var userActivity = new Activity
            {
                Details = activity.Details,
                Name = activity.Name,
                Type = activity.Type.ToString(),
                StartedAt = DateTimeOffset.UtcNow,
                User = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                }
            };
            
            await _messageService.SendMessage(userActivity);
        }
    }

    // Dispose of the client when the service is disposed
    public async Task DisposeAsync()
    {
        await _client.DisposeAsync();
    }
    
    // Initialize the Discord client and connect to the gateway
    private async Task InitializeDiscordClient()
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Token);
        await _client.StartAsync();
    }
}