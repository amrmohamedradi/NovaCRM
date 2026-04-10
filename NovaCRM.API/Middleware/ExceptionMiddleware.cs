using System.Net;
using System.Text.Json;
using FluentValidation;
using NovaCRM.Application.Common;
using NovaCRM.Application.Exceptions;

namespace NovaCRM.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

            switch (ex)
            {
                case ValidationException:
                case InvalidOperationException:
                    logger.LogWarning(
                        "⚠ [{CorrelationId}] {ExceptionType}: {Message}",
                        correlationId, ex.GetType().Name, ex.Message);
                    break;

                case KeyNotFoundException:
                case UnauthorizedAccessException:
                case ForbiddenException:
                    logger.LogWarning(
                        "⚠ [{CorrelationId}] {ExceptionType}: {Message}",
                        correlationId, ex.GetType().Name, ex.Message);
                    break;

                default:
                    logger.LogError(ex,
                        "✗ [{CorrelationId}] Unhandled {ExceptionType}: {Message}",
                        correlationId, ex.GetType().Name, ex.Message);
                    break;
            }

            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context, Exception ex, string traceId)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;
        HttpStatusCode statusCode;

        switch (ex)
        {
            case ValidationException ve:
                statusCode = HttpStatusCode.BadRequest;

                var fieldErrors = ve.Errors
                    .Select(e => new FieldError(
                        Field:   ToCamelCase(e.PropertyName),
                        Message: e.ErrorMessage))
                    .ToList();
                response = ApiResponse<object>.ValidationFail(fieldErrors);
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                response   = ApiResponse<object>.Fail(ex.Message);
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                response   = ApiResponse<object>.Fail("Authentication required.");
                break;

            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                response   = ApiResponse<object>.Fail(ex.Message);
                break;

            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                response   = ApiResponse<object>.Fail(ex.Message);
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                response   = ApiResponse<object>.Fail(
                    "An unexpected error occurred. Please contact support.");
                break;
        }

        response.TraceId = traceId;
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, _jsonOpts));
    }

    private static string ToCamelCase(string name) =>
        string.IsNullOrEmpty(name)
            ? name
            : char.ToLowerInvariant(name[0]) + name[1..];
}
