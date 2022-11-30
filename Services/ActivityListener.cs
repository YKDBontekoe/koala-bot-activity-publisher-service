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
        _client.PresenceUpdated += Client_PresenceUpdated;
    }

    private async Task Client_PresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
    {
        if (user.IsBot) return;
        
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
                Username = user.Username,
            };
            await _messageService.SendMessage(userActivity);
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