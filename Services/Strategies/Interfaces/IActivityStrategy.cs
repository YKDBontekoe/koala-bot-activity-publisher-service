using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using Activity = Koala.ActivityPublisherService.Models.Activities.Activity;


namespace Koala.ActivityPublisherService.Services.Strategies.Interfaces;

public interface IActivityStrategy
{
    Activity CreateActivity(SocketUser user, IActivity activity);
    Type GetActivityType();
}
