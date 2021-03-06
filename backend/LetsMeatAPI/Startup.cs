using Google.Apis.Auth;
using LetsMeatAPI.Controllers;
using LetsMeatAPI.ExternalAPI;
using LetsMeatAPI.ReceiptExtractor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

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
               .LogTo(s => LogsController.AddLog(s), LogLevel.Information)
               .EnableSensitiveDataLogging()
      );
      services.AddSingleton<RNGCryptoServiceProvider>();
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
      services.AddScoped<IGoogleVision, GoogleVision>(
        s => new GoogleVision(
          _configuration.GetConnectionString("VISION_API_CREDENTIALS"),
          s.GetService<ILogger<GoogleVision>>()!
        )
      );
      services.AddScoped<IUriReceiptExtractor, GoogleVisionReceiptExtractor>(
        s => new GoogleVisionReceiptExtractor(
          s.GetService<ITextReceiptExtractor>()!,
          s.GetService<IGoogleVision>()!
        )
      );
      services.AddScoped<ITextReceiptExtractor, PLMcDonaldsReceiptExtractor>(
        s => new PLMcDonaldsReceiptExtractor(new EmptyReceiptExtractor())
      );
      services.AddScoped<IDebtReducer, DebtReducer>();
      services.AddSingleton<IUserManager, UserManager>();
      services.AddScoped<IPaidResourceGuard, PaidResouceGuard>(
        s => new PaidResouceGuard(0_250, s.GetService<ILogger<PaidResouceGuard>>()!)
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
      LMDbContext context,
      IUserManager userManager
    ) {
      app.UseMiddleware<RequestResponseLoggingMiddleware>();
      if(env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      app.UseCors(_letsMeatAPIPolicy);
      app.UseHttpsRedirection();
      app.UseRouting();
      var contentTypes = new FileExtensionContentTypeProvider();
      contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
      app.UseStaticFiles(new StaticFileOptions {
        ContentTypeProvider = contentTypes
      });
      app.UseEndpoints(endpoints => endpoints.MapControllers());
      app.UseSwagger();
      app.UseSwaggerUI(config => {
        config.SwaggerEndpoint("swagger/v1/swagger.json", "LetsMeat API Documentation");
        config.RoutePrefix = string.Empty;
      });

      foreach(var user in from user in context.Users
                          where user.Token != null
                          select user) {
        userManager.LogIn(user.Token!, user.Id);
      }
    }
    private readonly IConfiguration _configuration;
    private const string _letsMeatAPIPolicy = "_letsMeatAPIPolicy";
  }
}
