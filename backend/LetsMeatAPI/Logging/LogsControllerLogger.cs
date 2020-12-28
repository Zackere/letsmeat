using LetsMeatAPI.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace LetsMeatAPI {
  public class LogsControllerLoggerProvider : ILoggerProvider {
    public ILogger CreateLogger(string categoryName) {
      return _loggers.GetOrAdd(categoryName, name => new LogsControllerLogger(name));
    }
    public void Dispose() {
      _loggers.Clear();
    }
    private readonly ConcurrentDictionary<string, LogsControllerLogger> _loggers = new();
  }
  public class LogsControllerLogger : ILogger {
    public LogsControllerLogger(string name) {
      _name = name;
    }
    public void Log<TState>(
      LogLevel logLevel,
      EventId eventId,
      TState state,
      Exception exception,
      Func<TState, Exception, string> formatter
    ) {
      LogsController.AddLog($"{logLevel} from {_name} at {DateTime.Now}: {formatter(state, exception)}");
    }
    public bool IsEnabled(LogLevel logLevel) {
      return true;
    }
    public IDisposable? BeginScope<TState>(TState state) {
      return default;
    }
    private readonly string _name;
  }
}
