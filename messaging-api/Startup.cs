using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SarData.Messaging.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
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
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          string authority = Configuration["auth:authority"].TrimEnd('/');
          logger.LogInformation("JWT Authority {0}", authority);
          options.Authority = authority;
          options.Audience = $"{authority}/resources";
          options.RequireHttpsMetadata = environment != EnvironmentName.Development;
        });

      services.SetupSmtp(Configuration, appRoot, logger);
      services.SetupSms(Configuration, logger);

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
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
      app.UseMvc();
    }
  }
}
