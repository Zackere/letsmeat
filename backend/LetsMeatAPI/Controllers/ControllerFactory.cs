using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LetsMeatAPI {
  public class ControllerFactory : IControllerActivator {
    public ControllerFactory(
      IWebHostEnvironment webHostEnvironment,
      IEnumerable<string>? expectedGoogleAudiences,
      IServiceProvider serviceProvider
    ) {
      _webHostEnvironment = webHostEnvironment;
      _expectedGoogleAudiences = expectedGoogleAudiences;
      _serviceProvider = serviceProvider;
    }
    public object Create(ControllerContext context) {
      var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
      object ret;
      if(controllerType == typeof(LoginController)) {
        var loginController = new LoginController(
          Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync,
          _expectedGoogleAudiences,
          _webHostEnvironment,
          new(),
          _serviceProvider.GetService<ILogger<LoginController>>()!
        );
        loginController.OnTokenGranted += (token, jwt) => {
          using var scope = _serviceProvider.CreateScope();
          var userManager = scope.ServiceProvider.GetService<UserManager>();
          userManager?.OnTokenGranted(token, jwt);
        };

        ret = loginController;
      } else {
        ret = Activator.CreateInstance(controllerType)
            ?? throw new ArgumentException($"Cannot not create controller of type {controllerType.Name}");
      }
      return ret;
    }
    public void Release(ControllerContext context, object controller) {
      (controller as IDisposable)?.Dispose();
    }
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEnumerable<string>? _expectedGoogleAudiences;
    private readonly IServiceProvider _serviceProvider;
  }
}
