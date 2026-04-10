namespace NovaCRM.Application.Common;

public class ApiResponse<T>
{
    public bool   Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T?     Data    { get; set; }

    public List<string>     Errors      { get; set; } = [];

    public List<FieldError> FieldErrors { get; set; } = [];

    public string? TraceId { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Created(T data, string message = "Created successfully") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? [] };

    public static ApiResponse<T> Fail(List<string> errors) =>
        new() { Success = false, Message = "Validation failed.", Errors = errors };

    public static ApiResponse<T> ValidationFail(List<FieldError> fieldErrors) =>
        new()
        {
            Success     = false,
            Message     = "One or more validation errors occurred.",
            Errors      = fieldErrors.Select(e => $"{e.Field}: {e.Message}").ToList(),
            FieldErrors = fieldErrors
        };
}
