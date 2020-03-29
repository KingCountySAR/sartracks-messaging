using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SarData.Messaging.Api.Health
{
  public class TwilioHealthCheck : IHealthCheck
  {
    List<string> ignoreComponents = new List<string> { "SMS, Latin America", "SMS, APAC", "SMS, Europe", "SMS, Middle East & Africa", "Voice, Latin America", "Voice, APAC", "Voice, Europe", "Voice, Middle East & Africa" };

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      try
      {
        HealthStatus status = HealthStatus.Healthy;

        HttpClient http = new HttpClient();
        var response = JsonSerializer.Deserialize<JsonElement>(await http.GetStringAsync("https://gpkpyklzq55q.statuspage.io/api/v2/summary.json"));
        foreach (var component in response.GetProperty("components").EnumerateArray())
        {
          string name = component.GetProperty("name").GetString() ?? "nothing";
          if (ignoreComponents.Contains(name)) continue;

          string statusText = component.GetProperty("status").GetString();
          if (statusText == "operational")
          {
            // do nothing
          }
          else if (statusText.Contains("degraded") && status == HealthStatus.Healthy)
          {
            status = HealthStatus.Degraded;
          }
        }

        data.Add("_result", status);
        return new HealthCheckResult(status == HealthStatus.Healthy ? HealthStatus.Healthy : context.Registration.FailureStatus);
      }
      catch (Exception e)
      {
        data.Add("_result", HealthStatus.Unhealthy);
        return new HealthCheckResult(
          context.Registration.FailureStatus,
          exception: e,
          data: data
          );
      }
    }
  }
}
