using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Strategies.Interfaces;

namespace Koala.ActivityPublisherService.Services.Strategies;

public class SpotifyActivityStrategy : IActivityStrategy
{
    public Activity CreateActivity(SocketUser user, IActivity activity)
    {
        var spotifyGame = (SpotifyGame)activity;
        return new SpotifyActivity
        {
            SpotifyInfo = CreateSpotifyInfo(spotifyGame),
            Details = GetSpotifyGameDetails(spotifyGame),
            Name = activity.Name,
            Type = activity.Type.ToString(),
            StartedAt = DateTimeOffset.UtcNow,
            User = CreateUser(user)
        };
    }

    public Type GetActivityType()
    {
        return typeof(SpotifyActivity);
    }

    private static SpotifyInfo CreateSpotifyInfo(SpotifyGame spotifyGame)
    {
        return new SpotifyInfo
        {
            Album = spotifyGame.AlbumTitle,
            Artists = spotifyGame.Artists,
            Track = spotifyGame.TrackTitle,
            TrackId = spotifyGame.TrackId,
            DurationInSeconds = spotifyGame.Duration is { TotalSeconds: > 0 }
                ? (int)spotifyGame.Duration.Value.TotalSeconds
                : null,
        };
    }

    private static string GetSpotifyGameDetails(SpotifyGame spotifyGame)
    {
        var artists = string.Join(", ", spotifyGame.Artists);
        return $"Listening to {artists} - {spotifyGame.TrackTitle}";
    }

    private static User CreateUser(SocketUser user)
    {
        return new User
        {
            Id = user.Id,
            Username = user.Username,
            NickName = user is SocketGuildUser guildUser ? guildUser.Nickname : null,
            Guilds = user.MutualGuilds.Select(x => new Guild
            {
                Id = x.Id,
                Name = x.Name,
            })
        };
    }
}