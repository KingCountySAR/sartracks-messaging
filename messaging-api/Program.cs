using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SarData.Messaging.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      var builder = WebHost.CreateDefaultBuilder(args);
      Console.WriteLine("Process ID: " + System.Diagnostics.Process.GetCurrentProcess().Id);
      return builder
        .UseStartup<Startup>()
        .ConfigureAppConfiguration(config =>
        {
          config.AddJsonFile("appsettings.json", true, false)
                .AddJsonFile("appsettings.local.json", true, false)
                .AddEnvironmentVariables();
        })
        .Build();
    }
  }
}
