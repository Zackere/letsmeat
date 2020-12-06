using Google.Apis.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace LetsMeatAPI {
  public class Startup {
    public Startup(IConfiguration configuration) {
      _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services) {
      services.AddDbContext<LMDbContext>(options =>
        options.UseSqlServer(_configuration.GetConnectionString("LMDatabase"))
               .UseLazyLoadingProxies()
      );
      services.AddScoped<Random>();
      services.AddScoped<UserManager>();
      services.AddScoped<Controllers.LoginController.GoogleTokenIdValidator>(
        _ => GoogleJsonWebSignature.ValidateAsync
      );
      services.AddScoped(_ => new Controllers.LoginController.GoogleAudiences {
        Auds = from aud in new[] {
                Environment.GetEnvironmentVariable("WEB_GOOGLE_CLIENT_ID"),
                Environment.GetEnvironmentVariable("MOBILE_GOOGLE_CLIENT_ID")
               }
               where aud != null
               select aud
      });
      services.AddControllers();
      services.AddCors(cors => cors.AddPolicy(
        _letsMeatAPIPolicy,
        builder => builder
                  .AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
      ));
      services.AddSwaggerGen(config => {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        config.IncludeXmlComments(xmlPath);
        config.CustomSchemaIds(x => x.FullName);
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      app.UseMiddleware<Utils.RequestResponseLoggingMiddleware>();
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
    private readonly IConfiguration _configuration;
    private readonly string _letsMeatAPIPolicy = "_letsMeatAPIPolicy";
  }
}
