using System;
using System.IO;
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

      var message = new MailMessage(fromAddress, request.To, request.Subject, request.Message) { IsBodyHtml = true };
      if (request.Attachments != null)
      {
        foreach (var attachment in request.Attachments)
        {
          message.Attachments.Add(new Attachment(new MemoryStream(Convert.FromBase64String(attachment.Base64)), attachment.FileName, attachment.MimeType));
        }
      }
      await smtp.SendMailAsync(message);

      foreach (var forClosing in message.Attachments)
      {
        forClosing.ContentStream.Dispose();
      }
      return Ok(new { Data = new object { } });
    }
  }
}