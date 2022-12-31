using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Strategies.Interfaces;

namespace Koala.ActivityPublisherService.Services.Strategies;

public class ActivityStrategy : IActivityStrategy
{
    public Activity CreateActivity(SocketUser user, IActivity activity)
    {
        return new Activity
        {
            Type = activity.Type.ToString(),
            Name = activity.Name,
            StartedAt = DateTimeOffset.Now,
            User = CreateUser(user)
        };
    }

    public Type GetActivityType()
    {
        return typeof(Activity);
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