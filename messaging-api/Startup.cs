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
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;

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
      services.AddHealthChecks();

      var insightsKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
      if (!string.IsNullOrWhiteSpace(insightsKey))
      {
        services.AddApplicationInsightsTelemetry(insightsKey);
      }
      
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          string authority = Configuration["auth:authority"].TrimEnd('/');
          logger.LogInformation("JWT Authority {0}", authority);
          options.Authority = authority;
          options.Audience = $"{authority}/resources";
          options.RequireHttpsMetadata = environment != Environments.Development;
        });

      services.SetupSmtp(Configuration, appRoot, logger);
      services.SetupSms(Configuration, logger);

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHealthChecks("/_health", new HealthCheckOptions
      {
        ResponseWriter = async (context, report) =>
        {
          var result = JsonConvert.SerializeObject(
              new
              {
                status = report.Status.ToString(),
                errors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
              });
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
      app.UseAuthentication();
      app.UseRouting();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
