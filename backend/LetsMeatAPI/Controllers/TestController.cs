using Microsoft.AspNetCore.Mvc;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class TestController : ControllerBase {
    [HttpGet]
    public ActionResult Index(int code) {
      return new StatusCodeResult(code);
    }
  }
}
