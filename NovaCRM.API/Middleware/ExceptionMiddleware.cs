using System.Net;
using System.Text.Json;
using FluentValidation;
using NovaCRM.Application.Common;

namespace NovaCRM.API.Middleware;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = ex switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(ve.Errors.Select(e => e.ErrorMessage).ToList())),

            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                ApiResponse<object>.Fail(ex.Message)),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ApiResponse<object>.Fail("Unauthorized.")),

            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                ApiResponse<object>.Fail(ex.Message)),

            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse<object>.Fail("An unexpected error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}



