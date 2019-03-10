using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SarData.Messaging.Api
{
  public static class SmtpExtensions
  {
    public static void SetupSmtp(this IServiceCollection services, IConfiguration Configuration, string contentRoot, ILogger logger)
    {
      services.AddScoped(svcs =>
      {
        var config = svcs.GetRequiredService<IConfiguration>();

        string host = config.GetValue<string>("email:smtp:host");

        if (string.IsNullOrWhiteSpace(host))
        {
          logger.LogInformation($"SMTP - No configuration found. Writing emails to {contentRoot}");
          return GetLocalClient(contentRoot);
        }
        else
        {
          return GetSmtpClient(Configuration, logger);
        }
      });
    }

    private static SmtpClient GetSmtpClient(IConfiguration config, ILogger logger)
    {
      var host = config.GetValue<string>("email:smtp:host");
      var port = config.GetValue<int>("email:smtp:port");
      var username = config.GetValue<string>("email:smtp:username");
      logger.LogInformation($"SMTP - Will send email from {config.GetValue<string>("email:smtp:from")} to {host}:{port} as {username}");

      return new SmtpClient
      {
        Host = host,
        Port = port,
        EnableSsl = true,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(
                          username,
                          config.GetValue<string>("email:smtp:password")
                        )
      };
    }

    private static SmtpClient GetLocalClient(string contentRoot)
    {
      var directory = Path.Combine(contentRoot, "logs", "emails");
      Directory.CreateDirectory(directory);

      return new SmtpClient
      {
        DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
        PickupDirectoryLocation = directory
      };
    }
  }
}
