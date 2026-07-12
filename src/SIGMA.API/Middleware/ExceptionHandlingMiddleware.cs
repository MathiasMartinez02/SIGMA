using System.Net;
using System.Text.Json;
using FluentValidation;
using SIGMA.Domain.Exceptions;

namespace SIGMA.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.UnprocessableEntity,
                "Validation Error",
                ve.Errors.Select(e => e.ErrorMessage).ToArray()),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                "Not Found",
                new[] { nfe.Message }),

            DomainException de => (
                HttpStatusCode.BadRequest,
                "Business Rule Violation",
                new[] { de.Message }),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                new[] { "No está autorizado para realizar esta acción." }),

            _ => (
                HttpStatusCode.InternalServerError,
                "Server Error",
                new[] { "Ocurrió un error interno. Por favor intente nuevamente." })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            success = false,
            message = title,
            errors
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
