using Azure.Messaging.ServiceBus;
using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Options;
using Koala.ActivityPublisherService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

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
        _client.PresenceUpdated += SendActivitiesUpdates;
        _client.PresenceUpdated += SendStatusUpdates;
    }

    private async Task SendActivitiesUpdates(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user.IsBot)
        {
            return;
        }
        foreach (var activity in newPresence.Activities)
        {
            Activity userActivity = new ();
            var details = activity.Details;
            if (activity is { Type: ActivityType.Listening, Name: "Spotify" } and SpotifyGame spotifyGame)
            {
                userActivity.SpotifyInfo = new SpotifyInfo
                {
                    Album = spotifyGame.AlbumTitle,
                    Artists = spotifyGame.Artists,
                    Track = spotifyGame.TrackTitle,
                    DurationInSeconds = spotifyGame.Duration is { TotalSeconds: > 0 }
                        ? (int)spotifyGame.Duration.Value.TotalSeconds
                        : null,
                };
                details = GetSpotifyGameDetails(spotifyGame);
            }

            userActivity.Details = details;
            userActivity.Name = activity.Name;
            userActivity.Type = activity.Type.ToString();
            userActivity.StartedAt = DateTimeOffset.UtcNow;
            userActivity.User = new User
            {
                Id = user.Id,
                Username =user.Username,
                NickName = user is SocketGuildUser guildUser ? guildUser.Nickname : null,
                Guilds = user.MutualGuilds.Select(x => new Guild
                {
                    Id = x.Id,
                    Name = x.Name,
                })
            };
            await _messageService.SendMessage(userActivity);
        }
    }

    private async Task SendStatusUpdate(IUser user, string statusType, string statusDetails)
    {
        await _messageService.SendMessage(new Activity
        {
            User = new User
            {
                Id = user.Id,
                Username = user.Username,
            },
            StartedAt = DateTimeOffset.UtcNow,
            Type = statusType,
            Details = user.Username + " is " + statusDetails,
            Name = statusDetails,
        });
    }

    private async Task SendOfflineUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "Status", "Offline");
    }

    private async Task SendOnlineUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "Status", "Online");
    }

    private async Task SendDoNotDisturbUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "Status", "Do Not Disturb");
    }

    private async Task SendIdleUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "Idle", "Idle");
    }

    private async Task SendInvisibleUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "Invisible", "Invisible");
    }

    private async Task SendAfkUpdate(SocketUser user)
    {
        await SendStatusUpdate(user, "AFK", "AFK");
    }

    private async Task SendStatusUpdates(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user.IsBot)
        {
            return;
        }
        switch (newPresence.Status)
        {
            case UserStatus.Offline:
                await SendOfflineUpdate(user);
                break;
            case UserStatus.Online:
                await SendOnlineUpdate(user);
                break;
            case UserStatus.DoNotDisturb:
                await SendDoNotDisturbUpdate(user);
                break;
            case UserStatus.Idle:
                await SendIdleUpdate(user);
                break;
            case UserStatus.Invisible:
                await SendInvisibleUpdate(user);
                break;
            case UserStatus.AFK:
                await SendAfkUpdate(user);
                break;
            default:
                Console.WriteLine("Unknown status");
                break;
        }
    }

    private static string GetSpotifyGameDetails(SpotifyGame spotifyGame)
    {
        var artists = string.Join(", ", spotifyGame.Artists);
        return $"Listening to {artists} - {spotifyGame.TrackTitle}";
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