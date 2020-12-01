using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LogsController : ControllerBase {
    [HttpGet]
    public ActionResult<IEnumerable<string>> Index() {
      return Directory.GetFiles("Logs");
    }
    [Route("{filename}")]
    [HttpGet]
    public string Read(string filename) {
      using var sr = new StreamReader($"Logs/{filename}");
      return sr.ReadToEnd();
    }
  }
}
