using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace LetsMeatAPI.Logging {
  public sealed class LogEmitterProvider : ILoggerProvider {
    public LogEmitterProvider(LogLevel logLevel) {
      _logLevel = logLevel;
    }

    public ILogger CreateLogger(string categoryName) {
      return _loggers.GetOrAdd(categoryName, name => new LogEmitter(name, _logLevel));
    }

    public void Dispose() {
      _loggers.Clear();
    }

    private readonly ConcurrentDictionary<string, LogEmitter> _loggers = new();
    private readonly LogLevel _logLevel;
  }
  public class LogEmitter : ILogger {
    public LogEmitter(
      string name,
      LogLevel logLevel
    ) {
      _name = name;
      _logLevel = logLevel;
    }
    public IDisposable? BeginScope<TState>(TState state) {
      return default;
    }
    public bool IsEnabled(LogLevel logLevel) {
      return logLevel >= _logLevel;
    }
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter) {
      if(!IsEnabled(logLevel)) {
        return;
      }
      _mtx.WaitOne();
      using var sw = File.AppendText("logs.txt");
      sw.WriteLine(formatter(state, exception));
      _mtx.ReleaseMutex();
    }
    private static readonly Mutex _mtx = new();
    private readonly string _name;
    private readonly LogLevel _logLevel;
  }
}
