using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using SarData.Common.Apis.Messaging;

namespace SarData.Messaging.Api.Controllers
{
  [Authorize]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly SmtpClient smtp;
    private readonly string fromAddress;

    public EmailController(SmtpClient smtp, IConfiguration config)
    {
      this.smtp = smtp;
      fromAddress = config.GetValue<string>("email:smtp:from") ?? "server@example.com";
    }

    // GET api/values/5
    [HttpPost("/send/email")]
    public async Task<IActionResult> SendEmail([FromBody, BindRequired] SendEmailRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { Error = "Invalid request" });
      }

      await smtp.SendMailAsync(new MailMessage(fromAddress, request.To, request.Subject, request.Message));
      return Ok(new { Data = new object { } });
    }
  }
}