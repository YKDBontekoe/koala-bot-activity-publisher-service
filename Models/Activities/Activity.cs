namespace Koala.ActivityPublisherService.Models.Activities;

public class Activity
{
    public required string Name { get; init; } = string.Empty;
    public required string Type { get; init; } = string.Empty;
    public required DateTimeOffset StartedAt { get; init; } = DateTimeOffset.Now;
    public string Details { get; init; } = string.Empty;
    public required User User { get; init; } = new ();
}