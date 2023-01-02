using Discord;
using Discord.WebSocket;
using Koala.ActivityPublisherService.Options;
using Koala.ActivityPublisherService.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Koala.ActivityPublisherService;

public class ActivityPublisherWorker : IHostedService
{
    private readonly IActivityListener _activityListener;
    private readonly IStatusListener _statusListener;
    private readonly DiscordSocketClient _discordClient;
    private readonly DiscordOptions _discordOptions;

    public ActivityPublisherWorker(IActivityListener activityListener, IStatusListener statusListener, DiscordSocketClient discordClient, IOptions<DiscordOptions> discordOptions)
    {
        _activityListener = activityListener;
        _statusListener = statusListener;
        _discordClient = discordClient;
        _discordOptions = discordOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordClient.LoginAsync(TokenType.Bot, _discordOptions.Token);
        await _discordClient.StartAsync();
        await _activityListener.InitializeAsync();
        await _statusListener.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _activityListener.DisposeAsync();
        await _statusListener.DisposeAsync();
    }
}