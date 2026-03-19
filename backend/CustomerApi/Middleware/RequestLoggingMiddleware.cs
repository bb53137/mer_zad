using System.Diagnostics;

namespace CustomerApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        Console.WriteLine(
            $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds} ms");
    }
}