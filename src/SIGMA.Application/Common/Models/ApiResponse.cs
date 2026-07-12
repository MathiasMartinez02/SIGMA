namespace SIGMA.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public string[]? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string[] errors, string? message = null) =>
        new() { Success = false, Errors = errors, Message = message };

    public static ApiResponse<T> Fail(string error) =>
        new() { Success = false, Errors = [error] };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string[]? Errors { get; init; }

    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string error) =>
        new() { Success = false, Errors = [error] };
}
