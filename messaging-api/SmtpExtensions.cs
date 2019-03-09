using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SarData.Messaging.Api
{
  public static class SmtpExtensions
  {
    public static void SetupSmtp(this IServiceCollection services, IConfiguration Configuration, string contentRoot)
    {
      services.AddScoped(svcs =>
      {
        var config = svcs.GetRequiredService<IConfiguration>();

        string host = config.GetValue<string>("email:smtp:host");

        return string.IsNullOrWhiteSpace(host) ? GetLocalClient(contentRoot) : GetSmtpClient(Configuration);
      });
    }

    private static SmtpClient GetSmtpClient(IConfiguration config)
    {
      return new SmtpClient
      {
        Host = config.GetValue<string>("email:smtp:host"),
        Port = config.GetValue<int>("email:smtp:port"),
        EnableSsl = true,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(
                          config.GetValue<string>("email:smtp:username"),
                          config.GetValue<string>("email:smtp:password")
                        )
      };
    }

    private static SmtpClient GetLocalClient(string contentRoot)
    {
      return new SmtpClient
      {
        DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
        PickupDirectoryLocation = Path.Combine(contentRoot, "emails")
      };
    }
  }
}
