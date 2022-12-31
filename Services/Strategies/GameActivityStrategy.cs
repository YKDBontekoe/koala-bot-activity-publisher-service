using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Constants;
using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;
using Koala.ActivityPublisherService.Services.Strategies.Interfaces;
using GameParty = Koala.ActivityPublisherService.Models.GameParty;

namespace Koala.ActivityPublisherService.Services.Strategies;

public class GameActivityStrategy : IActivityStrategy
{
    public Activity CreateActivity(SocketUser user, IActivity activity)
    {
        var gameActivity = (RichGame)activity;
        return new GameActivity
        {
            Name = gameActivity.Name,
            Type = MessageTypes.Playing,
            Details = gameActivity.Details,
            GameInfo = CreateGameInfo(gameActivity),
            User = CreateUser(user),
            StartedAt = gameActivity.Timestamps.Start!.Value
        };
    }

    public Type GetActivityType()
    {
        return typeof(GameActivity);
    }

    private static GameInfo CreateGameInfo(RichGame gameActivity)
    {
        return new GameInfo
        {
            ApplicationId = gameActivity.ApplicationId,
            Party = CreateGameParty(gameActivity),
            Timestamps = CreateGameTimestamps(gameActivity),
        };
    }

    private static GameParty CreateGameParty(RichGame gameActivity)
    {
        return new GameParty
        {
            Id = gameActivity.Party.Id,
            Size = CreateActivityPartySize(gameActivity),
        };
    }

    private static GameParty.ActivityPartySize CreateActivityPartySize(RichGame gameActivity)
    {
        return new GameParty.ActivityPartySize
        {
            Id = gameActivity.Party.Id,
            Members = gameActivity.Party.Members,
            Capacity = gameActivity.Party.Capacity,
        };
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
    
    private static GameTimeStamps CreateGameTimestamps(RichGame gameActivity)
    {
        return new GameTimeStamps
        {
            Start = gameActivity.Timestamps.Start,
            End = gameActivity.Timestamps.End,
        };
    }
}