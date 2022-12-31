using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Interfaces;
using Koala.ActivityPublisherService.Services.Strategies;
using Koala.ActivityPublisherService.Services.Strategies.Interfaces;

namespace Koala.ActivityPublisherService.Services;

public class ActivityListener : IActivityListener
{
    private readonly DiscordSocketClient _client;
    private readonly IMessageService _messageService;
    private readonly Dictionary<ActivityType, IActivityStrategy> _strategies;

    public ActivityListener(DiscordSocketClient client, IMessageService messageService)
    {
        _client = client;
        _messageService = messageService;
        _strategies = new Dictionary<ActivityType, IActivityStrategy>
        {
            { ActivityType.Listening, new SpotifyActivityStrategy() },
            { ActivityType.Playing, new GameActivityStrategy() },
            { ActivityType.Streaming, new StreamingActivityStrategy() }
        };
    }

    public Task InitializeAsync()
    {
        _client.PresenceUpdated += SendActivitiesUpdates;
        return Task.CompletedTask;
    }
    
    public async Task DisposeAsync()
    {
        await _client.DisposeAsync();
    }
    
    private async Task SendActivitiesUpdates(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user.IsBot)
        {
            return;
        }

        foreach (var activity in newPresence.Activities)
        {
            var strategy = _strategies.GetValueOrDefault(activity.Type, new ActivityStrategy());
            var userActivity = strategy.CreateActivity(user, activity);
            await _messageService.SendMessage(userActivity, strategy.GetActivityType());
        }
    }
}


