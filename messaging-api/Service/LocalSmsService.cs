using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SarData.Messaging.Api.Service
{
  public class LocalSmsService : ISmsService
  {
    private readonly ILogger<ISmsService> logger;
    private readonly string smsFile;

    public LocalSmsService(IHostingEnvironment env, ILogger<ISmsService> logger)
    {
      var directory = Path.Combine(env.ContentRootPath, "logs");
      Directory.CreateDirectory(directory);
      smsFile = Path.Combine(directory, "sms.log");
      this.logger = logger;
    }

    public virtual async Task SendMessage(string to, string message)
    {
      logger.LogInformation("Sending SMS to {0}", to);
      await File.AppendAllTextAsync(smsFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:ii:ss")} - {to} - {message}\\n");
    }
  }
}
