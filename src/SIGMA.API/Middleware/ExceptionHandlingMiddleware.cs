using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private const int SqlUniqueIndexViolation = 2601;
    private const int SqlUniqueConstraintViolation = 2627;
    private const int SqlForeignKeyViolation = 547;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

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

    // Respuesta de error con el mismo formato que ApiResponse
    public static Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string title, params string[] errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var json = JsonSerializer.Serialize(new { success = false, message = title, errors }, JsonOptions);
        return context.Response.WriteAsync(json);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
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

            DbUpdateConcurrencyException => (
                HttpStatusCode.Conflict,
                "Conflict",
                new[] { "Otro usuario modificó estos datos al mismo tiempo. Recargá la página e intentá de nuevo." }),

            DbUpdateException { InnerException: SqlException { Number: SqlUniqueIndexViolation or SqlUniqueConstraintViolation } } => (
                HttpStatusCode.Conflict,
                "Conflict",
                new[] { "Ya existe un registro con esos datos." }),

            DbUpdateException { InnerException: SqlException { Number: SqlForeignKeyViolation } } => (
                HttpStatusCode.Conflict,
                "Conflict",
                new[] { "La operación no se puede realizar porque el registro está relacionado con otros datos." }),

            _ => (
                HttpStatusCode.InternalServerError,
                "Server Error",
                new[] { "Ocurrió un error interno. Por favor intente nuevamente." })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            _logger.LogWarning("{Title} ({StatusCode}) en {Path}: {Errors}", title, (int)statusCode, context.Request.Path, string.Join(" | ", errors));

        await WriteErrorAsync(context, statusCode, title, errors);
    }
}
