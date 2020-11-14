using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
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
      services.AddDbContext<LMDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("LMDatabase"))
               .UseLazyLoadingProxies()
      );
      services.AddSingleton<IControllerActivator, ControllerFactory>(
        serviceProvider => new ControllerFactory(
            _webHostEnvironment,
            from aud in new[] {
              Environment.GetEnvironmentVariable("WEB_GOOGLE_CLIENT_ID"),
              Environment.GetEnvironmentVariable("MOBILE_GOOGLE_CLIENT_ID")
            }
            where aud != null
            select aud,
            serviceProvider
          )
      );
      services.AddControllers();
      services.AddCors(cors => cors.AddPolicy(
        _letsMeatAPIPolicy,
        builder => builder.AllowAnyOrigin().AllowAnyMethod()
      ));
      services.AddSwaggerGen(config => {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        config.IncludeXmlComments(xmlPath);
      });

      services.AddTransient<UserManager>();
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
      app.UseSwagger();
      app.UseSwaggerUI(config => {
        config.SwaggerEndpoint("swagger/v1/swagger.json", "LetsMeat API Documentation");
        config.RoutePrefix = string.Empty;
      });
    }
  }
}
