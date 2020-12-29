using Google.Apis.Auth;
using LetsMeatAPI.Controllers;
using LetsMeatAPI.ExternalAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace LetsMeatAPI {
  public class Startup {
    public Startup(IConfiguration configuration) {
      _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services) {
      services.AddHttpClient();
      services.AddDbContext<LMDbContext>(options =>
        options.UseSqlServer(_configuration.GetConnectionString("LMDatabase"))
               .UseLazyLoadingProxies()
      );
      services.AddScoped<Random>();
      services.AddScoped<ILocationCritic, LocationCritic>();
      services.AddScoped<IElectionHolder, ElectionHolder>();
      services.AddScoped<IGooglePlaces, GooglePlaces>(
        s => new GooglePlaces(
          Environment.GetEnvironmentVariable("PLACES_API_KEY") ?? "PLACES_API_KEY",
          s.GetService<IHttpClientFactory>()!,
          s.GetService<ILogger<GooglePlaces>>()!
        )
      );
      services.AddScoped<IDebtReducer, DebtReducer>();
      services.AddScoped<IUserManager, UserManager>();
      services.AddScoped<IPaidResourceGuard, PaidResouceGuard>(
        s => new PaidResouceGuard(2_000, s.GetService<ILogger<PaidResouceGuard>>()!)
      );
      services.AddScoped<LoginController.GoogleTokenIdValidator>(
        _ => GoogleJsonWebSignature.ValidateAsync
      );
      services.AddScoped(_ => new LoginController.GoogleAudiences {
        Auds = from aud in new[] {
                Environment.GetEnvironmentVariable("WEB_GOOGLE_CLIENT_ID"),
                Environment.GetEnvironmentVariable("MOBILE_GOOGLE_CLIENT_ID")
               }
               where aud != null
               select aud
      });
      services.AddScoped<IBlobClientFactory, BlobClientFactory>(
        _ => new BlobClientFactory(
          _configuration.GetConnectionString("LMBlobStorage") ??
          Environment.GetEnvironmentVariable("LMBlobStorage") ??
          throw new ArgumentNullException("Could not retrieve LMBlobStorage conn string")
        )
      );
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
    public void Configure(
      IApplicationBuilder app,
      IWebHostEnvironment env,
      LMDbContext context
    ) {
      app.UseMiddleware<RequestResponseLoggingMiddleware>();
      if(env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      app.UseCors(_letsMeatAPIPolicy);
      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
      app.UseSwagger();
      app.UseSwaggerUI(config => {
        config.SwaggerEndpoint("swagger/v1/swagger.json", "LetsMeat API Documentation");
        config.RoutePrefix = string.Empty;
      });

      UserManager.Init(from user in context.Users
                       where user.Token != null
                       select new ValueTuple<string, string>(user.Token!, user.Id));
    }
    private readonly IConfiguration _configuration;
    private const string _letsMeatAPIPolicy = "_letsMeatAPIPolicy";
  }
}
