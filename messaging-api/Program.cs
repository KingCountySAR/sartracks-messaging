using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SarData.Logging;

namespace SarData.Messaging.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine("Messaging API process " + System.Diagnostics.Process.GetCurrentProcess().Id);
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      var builder = WebHost.CreateDefaultBuilder(args);
      var insightsKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
      if (!string.IsNullOrWhiteSpace(insightsKey))
      {
        builder = builder.UseApplicationInsights(insightsKey);
      }

      string contentRoot = "";

      return builder
        .UseStartup<Startup>()
        .ConfigureAppConfiguration((context, config) =>
        {
          config.AddConfigFiles(context.HostingEnvironment.EnvironmentName);
          contentRoot = context.HostingEnvironment.ContentRootPath;
        })
        .ConfigureLogging(logBuilder =>
        {
          logBuilder.AddSarDataLogging(contentRoot);
        })
        .Build();
    }
  }
}
