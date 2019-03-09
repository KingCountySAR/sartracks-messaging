using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SarData.Messaging.Api.Service;

namespace SarData.Messaging.Api.Controllers
{
  [Authorize]
  [ApiController]
  public class SmsController : ControllerBase
  {
    private readonly ISmsService sms;

    public SmsController(ISmsService sms)
    {
      this.sms = sms;
    }

    // GET api/values/5
    [HttpPost("/send/text")]
    public async Task<IActionResult> SendEmail([FromQuery, BindRequired] string phone, [FromQuery, BindRequired] string message)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new { Error = "Invalid request" });
      }

      await sms.SendMessage(phone, message);
      return Ok(new { Data = new object { } });
    }
  }
}