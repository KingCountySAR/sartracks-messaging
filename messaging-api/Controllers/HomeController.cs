using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SarData.Common.Apis.Health;
using SarData.Server.Apis.Health;

namespace SarData.Messaging.Api.Controllers
{
  public class HomeController : Controller
  {
    [Authorize]
    [HttpGet("/_health/auth")]
    public HealthResponse Index()
    {
      return new HealthResponse(HealthStatusType.Healthy);
    }
  }
}