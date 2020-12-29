using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Linq;

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
    public LogsController(IConfiguration configuration) {
      _configuration = configuration;
    }
    [HttpGet]
    public string Index() {
      var ret = string.Join('\n', _queue);
      foreach(var s in new[] {
        Environment.GetEnvironmentVariable("PLACES_API_KEY"),
        Environment.GetEnvironmentVariable("WEB_GOOGLE_CLIENT_ID"),
        Environment.GetEnvironmentVariable("MOBILE_GOOGLE_CLIENT_ID"),
        _configuration.GetConnectionString("LMBlobStorage"),
        Environment.GetEnvironmentVariable("LMBlobStorage"),
        _configuration.GetConnectionString("LMDatabase"),
      }) {
        if(s != null)
          ret = ret.Replace(s, string.Concat(Enumerable.Repeat('*', s.Length)));
      }
      return ret;
    }
    private readonly IConfiguration _configuration;
  }
}
