using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LogsController : ControllerBase {
    [HttpGet]
    public string Index() {
      using var sr = new StreamReader("logs.txt");
      return sr.ReadToEnd();
    }
  }
}
