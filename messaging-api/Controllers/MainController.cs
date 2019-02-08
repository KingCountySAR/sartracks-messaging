using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarData.Common.Apis.Messaging;

namespace SarData.Messaging.Api.Controllers
{
  [Authorize]
  [ApiController]
  public class MainController : ControllerBase
  {

    // GET api/values/5
    [HttpPost("/send/email")]
    public IActionResult SendEmail([FromBody] SendEmailRequest request)
    {
      return Ok(new { Data = new object { } });
    }
  }
}
