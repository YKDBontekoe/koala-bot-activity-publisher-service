using Koala.ActivityPublisherService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityPublisherService;

public class ActivityPublisherWorker : IHostedService
{
    private readonly IActivityListener _activityListener;

    public ActivityPublisherWorker(IActivityListener activityListener)
    {
        _activityListener = activityListener;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _activityListener.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _activityListener.DisposeAsync();
    }
}