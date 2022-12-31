using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Constants;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Strategies.Interfaces;

namespace Koala.ActivityPublisherService.Services.Strategies;

public class StreamingActivityStrategy : IActivityStrategy
{
    public Activity CreateActivity(SocketUser user, IActivity activity)
    {
        var streamingActivity = (StreamingGame)activity;
        return new StreamingActivity
        {
            Type = MessageTypes.Streaming,
            Name = streamingActivity.Name,
            StartedAt = DateTimeOffset.Now,
            User = CreateUser(user),
            StreamingInfo = CreateStreamingInfo(streamingActivity)
        };
    }

    public Type GetActivityType()
    {
        return typeof(StreamingGame);
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
    
    private static StreamingInfo CreateStreamingInfo(StreamingGame streamingGame)
    {
        return new StreamingInfo
        {
            Url = streamingGame.Url,
        };
    }
}