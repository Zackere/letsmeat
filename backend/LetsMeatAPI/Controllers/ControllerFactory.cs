using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;

namespace LetsMeatAPI {
  public class ControllerFactory : IControllerActivator {
    public ControllerFactory(
      IWebHostEnvironment webHostEnvironment,
      IEnumerable<string> expectedGoogleAudiences
    ) {
      _webHostEnvironment = webHostEnvironment;
      _expectedGoogleAudiences = expectedGoogleAudiences;
    }
    public object Create(ControllerContext context) {
      Type controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
      object ret;
      switch(controllerType) {
      case Type LoginController:
        ret = new LoginController(
          Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync,
          _expectedGoogleAudiences,
          _webHostEnvironment,
          new Random()
        );
        break;
      default:
        ret = Activator.CreateInstance(controllerType)
              ?? throw new ArgumentException($"Cannot not create controller of type {controllerType.Name}");
        break;
      }
      return ret;
    }
    public void Release(ControllerContext context, object controller) {
      (controller as IDisposable)?.Dispose();
    }
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEnumerable<string> _expectedGoogleAudiences;
  }
}
