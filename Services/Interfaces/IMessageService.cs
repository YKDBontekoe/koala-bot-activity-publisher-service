using Koala.ActivityPublisherService.Models;
using Koala.ActivityPublisherService.Models.Activities;

namespace Koala.ActivityPublisherService.Services.Interfaces;

public interface IMessageService
{
    public Task SendMessage(Activity activity, Type type = null);
}