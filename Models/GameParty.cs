namespace Koala.ActivityPublisherService.Models;

public class GameParty
{
    public object Id { get; set; }
    public ActivityPartySize Size { get; set; }

    public class ActivityPartySize
    {
        public long Members { get; set; }
        public long Capacity { get; set; }
        public string Id { get; set; }
    }
}