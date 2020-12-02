using Microsoft.AspNetCore.Mvc;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LogsController : ControllerBase {
    [HttpGet]
    public string Index() {
      return string.Join('\n', Utils.RequestResponseLoggingMiddleware._queue);
    }
  }
}
