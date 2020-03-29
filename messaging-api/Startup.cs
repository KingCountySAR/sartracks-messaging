using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using SarData.Common.Apis.Health;
using SarData.Messaging.Api.Health;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;

namespace SarData.Messaging.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
    {
      Configuration = configuration;
      this.logger = logger;
      appRoot = env.ContentRootPath;
      environment = env.EnvironmentName;
    }

    public IConfiguration Configuration { get; }

    private readonly string appRoot;
    private readonly ILogger<Startup> logger;
    private readonly string environment;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var insightsKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
      if (!string.IsNullOrWhiteSpace(insightsKey))
      {
        services.AddApplicationInsightsTelemetry(insightsKey);
      }

      IdentityModelEventSource.ShowPII = (environment == Environments.Development);
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          string authority = Configuration["auth:authority"].TrimEnd('/');
          logger.LogInformation("JWT Authority {0}", authority);
          options.Authority = authority;
          options.Audience = $"{authority}/resources";
          options.TokenValidationParameters.ValidIssuer = authority;
          options.RequireHttpsMetadata = environment != Environments.Development;
        });
      
      services.SetupSmtp(Configuration, appRoot, logger);
      services.SetupSms(Configuration, logger);

      services.AddHealthChecks()
        .Add(new HealthCheckRegistration("sms", new TwilioHealthCheck(), HealthStatus.Degraded, new[] { "sms", "3rd-party" }));

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHealthChecks("/_health", new HealthCheckOptions
      {
        ResponseWriter = async (context, report) =>
        {
          var result = JsonSerializer.Serialize(
              new HealthResponse
              {
                Status = report.Status,
                Checks = report.Entries.Select(e => {
                  HealthStatus innerStatus = e.Value.Status;
                  if (e.Value.Data.TryGetValue("_result", out object statusObj))
                  {
                    innerStatus = (HealthStatus)statusObj;
                  }

                  return new HealthResponse.InnerCheck { Key = e.Key, Status = innerStatus };
                })
              },
              new JsonSerializerOptions().Setup());
          context.Response.ContentType = MediaTypeNames.Application.Json;
          await context.Response.WriteAsync(result);
        }
      });

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHttpsRedirection();
      }
      app.UseRouting()
         .UseAuthentication()
         .UseAuthorization()
         .UseEndpoints(endpoints =>
            {
              endpoints.MapControllers();
            });
    }
  }
}
