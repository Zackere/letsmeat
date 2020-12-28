using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace LetsMeatAPI.Controllers {
  [Route("[controller]")]
  [ApiController]
  public class LogsController : ControllerBase {
    private static readonly ConcurrentQueue<string> _queue = new();
    public static void AddLog(string log) {
      _queue.Enqueue(log);
      while(_queue.Count > 500)
        _queue.TryDequeue(out _);
    }
    [HttpGet]
    public string Index() {
      return string.Join('\n', _queue);
    }
  }
}
