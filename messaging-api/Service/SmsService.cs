using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;

namespace SarData.Messaging.Api.Service
{
  public class SmsService : NullSmsService
  {
    private readonly string fromNumber;

    public SmsService(IConfiguration config, ILogger<ISmsService> logger) : base(logger)
    {
      fromNumber = config["sms:twilio:from"];
    }

    public override async Task SendMessage(string to, string message)
    {
      to = Regex.Replace(to, "[^\\d]", "");
      if (to.Length != 10) throw new ArgumentException("unrecognized mobile number");

      var sms = await MessageResource.CreateAsync(
                  from: new Twilio.Types.PhoneNumber(fromNumber),
                  body: message,
                  to: new Twilio.Types.PhoneNumber($"+1${to}")
              );
    }
  }
}
