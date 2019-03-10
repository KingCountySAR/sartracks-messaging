using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SarData.Messaging.Api.Service;
using Twilio;

namespace SarData.Messaging.Api
{
  public static class SmsExtensions
  {
    public static void SetupSms(this IServiceCollection services, IConfiguration Configuration, ILogger logger)
    {
      string smsNumber = Configuration.GetValue<string>("sms:twilio:from");

      if (string.IsNullOrWhiteSpace(smsNumber))
      {
        logger.LogInformation("SMS - No configuration. Using null service");
        services.AddSingleton<ISmsService, LocalSmsService>();
      }
      else
      {
        var account = Configuration["sms:twilio:accountSid"];
        logger.LogInformation($"SMS - Will send from account {account} as {smsNumber}");
        TwilioClient.Init(account, Configuration["sms:twilio:authToken"]);
        services.AddSingleton<ISmsService, SmsService>();
      }
    }
  }
}
