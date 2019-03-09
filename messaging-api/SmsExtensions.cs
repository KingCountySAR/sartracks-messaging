using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SarData.Messaging.Api.Service;
using Twilio;

namespace SarData.Messaging.Api
{
  public static class SmsExtensions
  {
    public static void SetupSms(this IServiceCollection services, IConfiguration Configuration)
    {
      string smsNumber = Configuration.GetValue<string>("sms:twilio:from");

      if (string.IsNullOrWhiteSpace(smsNumber))
      {
        services.AddSingleton<ISmsService, NullSmsService>();
      }
      else
      {
        TwilioClient.Init(Configuration["sms:twilio:accountSid"], Configuration["sms:twilio:authToken"]);
        services.AddSingleton<ISmsService, SmsService>();
      }
    }
  }
}
