

namespace Koala.ActivityPublisherService.Models;

public class GameInfo
{
    public GameTimeStamps Timestamps { get; set; }
    public ulong ApplicationId { get; set; }
    public GameParty Party { get; set; }
}