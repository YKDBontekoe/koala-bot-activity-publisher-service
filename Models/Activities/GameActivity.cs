namespace Koala.ActivityPublisherService.Models.Activities;

public class GameActivity : Activity
{
    public required GameInfo GameInfo { get; set; }
}