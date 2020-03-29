using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SarData.Common.Apis.Health;

namespace SarData.Messaging.Api.Controllers
{
  public class HomeController : Controller
  {
    [Authorize]
    [HttpGet("/_health/auth")]
    public HealthResponse Index()
    {
      return new HealthResponse(HealthStatus.Healthy);
    }
  }
}