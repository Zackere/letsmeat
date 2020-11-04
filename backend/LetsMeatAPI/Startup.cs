using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace LetsMeatAPI {
  public class Startup {
    public Startup(IConfiguration configuration, IWebHostEnvironment env) {
      Configuration = configuration;
      _webHostEnvironment = env;
    }
    public IConfiguration Configuration { get; }
    private IWebHostEnvironment _webHostEnvironment { get; set; }
    private readonly string _letsMeatAPIPolicy = "_letsMeatAPIPolicy";
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddSingleton<IControllerActivator, ControllerFactory>(
        serviceProvider => new ControllerFactory(
            _webHostEnvironment,
            from aud in new[] {
              Environment.GetEnvironmentVariable("WEB_GOOGLE_CLIENT_ID"),
              Environment.GetEnvironmentVariable("MOBILE_GOOGLE_CLIENT_ID")
            }
            where aud != null
            select aud
          )
      );
      services.AddControllers();
      services.AddCors(cors => cors.AddPolicy(
        _letsMeatAPIPolicy,
        builder => builder.AllowAnyOrigin().AllowAnyMethod()
      ));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if(env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      app.UseCors(_letsMeatAPIPolicy);
      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthorization();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}
