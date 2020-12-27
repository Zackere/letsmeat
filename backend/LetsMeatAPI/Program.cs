using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace LetsMeatAPI {
  public class Program {
    public static void Main(string[] args) {
      var timer = new Timer(
        _ => PaidResouceGuard.DecayCounter(500),
        null,
        0,
        30_000
      );
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) {
      return Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
  }
}
