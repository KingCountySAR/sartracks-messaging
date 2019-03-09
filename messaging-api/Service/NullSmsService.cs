using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SarData.Messaging.Api.Service
{
  public class NullSmsService : ISmsService
  {
    private readonly ILogger<ISmsService> logger;

    public NullSmsService(ILogger<ISmsService> logger)
    {
      this.logger = logger;
    }

    public virtual Task SendMessage(string to, string message)
    {
      logger.LogInformation("Sending SMS to {0}", to);
      return Task.CompletedTask;
    }
  }
}
