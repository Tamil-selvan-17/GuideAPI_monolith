using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanMonolith.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new { message = exception.Message };
        int statusCode = (int)HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                break;
            case InvalidOperationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                break;
        }

        context.Response.StatusCode = statusCode;
        var result = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(result);
    }
}
