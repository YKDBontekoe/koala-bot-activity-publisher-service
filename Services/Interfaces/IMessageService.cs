using Koala.ActivityPublisherService.Models;

namespace Koala.ActivityPublisherService.Services.Interfaces;

public interface IMessageService
{
    public Task SendMessage(Activity activity);
}