using Newtonsoft.Json.Linq;

namespace Koala.ActivityPublisherService.Models;

public class Activity
{
    public string Type { get; set; } = "Activity";
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;

    public SpotifyInfo? SpotifyInfo { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    
    public User User { get; set; }
}