using LetsMeatAPI.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace LetsMeatAPI {
  public class Program {
    public static void Main(string[] args) {
      if(File.Exists("logs.txt"))
        File.Delete("logs.txt");
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) {
      return Host.CreateDefaultBuilder(args)
                 .ConfigureLogging(logging =>
                   logging.AddProvider(new LogEmitterProvider(LogLevel.Information))
                  )
                 .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
  }
}
