namespace Koala.ActivityPublisherService.Services.Interfaces;

public interface IActivityListener
{
    Task InitializeAsync();
    Task DisposeAsync();
}