using LetsMeatAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LetsMeatAPI {
  public class RequestResponseLoggingMiddleware {
    public RequestResponseLoggingMiddleware(
      RequestDelegate next
    ) {
      _next = next;
      _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }
    public async Task Invoke(HttpContext context) {
      if(
        context.Request.Path.Value == "/index.html" ||
        context.Request.Path.Value.Contains("swagger") ||
        context.Request.Path.Value == "/logs" ||
        context.Request.Path.Value == "/favicon.ico"
      ) {
        await _next(context);
        return;
      }
      LogsController.AddLog(await LogRequest(context));
      LogsController.AddLog(await LogResponse(context));
    }
    private async Task<string> LogRequest(HttpContext context) {
      context.Request.EnableBuffering();
      await using var requestStream = _recyclableMemoryStreamManager.GetStream();
      await context.Request.Body.CopyToAsync(requestStream);
      var ret = $"Http Request Information:\n" +
                $"Time: {DateTime.Now}\n" +
                $"Schema: {context.Request.Scheme}\n" +
                $"Method: {context.Request.Method}\n" +
                $"Host: {context.Request.Host}\n" +
                $"Path: {context.Request.Path}\n" +
                $"QueryString: {context.Request.QueryString}\n" +
                $"Request Body: {ReadStreamInChunks(requestStream)}\n";
      context.Request.Body.Position = 0;
      return ret;
    }
    private async Task<string> LogResponse(HttpContext context) {
      var originalBodyStream = context.Response.Body;
      await using var responseBody = _recyclableMemoryStreamManager.GetStream();
      context.Response.Body = responseBody;
      await _next(context);
      context.Response.Body.Seek(0, SeekOrigin.Begin);
      var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
      context.Response.Body.Seek(0, SeekOrigin.Begin);
      var ret = $"Http Response Information:\n" +
                $"Time: {DateTime.Now}\n" +
                $"Schema:{context.Request.Scheme}\n" +
                $"Host: {context.Request.Host}\n" +
                $"Path: {context.Request.Path}\n" +
                $"QueryString: {context.Request.QueryString}\n" +
                $"Response Code: {context.Response.StatusCode}\n" +
                $"Response Body: {text}\n";
      await responseBody.CopyToAsync(originalBodyStream);
      return ret;
    }
    private static string ReadStreamInChunks(Stream stream) {
      const int readChunkBufferLength = 4096;
      stream.Seek(0, SeekOrigin.Begin);
      using var textWriter = new StringWriter();
      using var reader = new StreamReader(stream);
      var readChunk = new char[readChunkBufferLength];
      int readChunkLength;
      do {
        readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
        textWriter.Write(readChunk, 0, readChunkLength);
      } while(readChunkLength > 0);
      return textWriter.ToString();
    }
    private readonly RequestDelegate _next;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
  }
}