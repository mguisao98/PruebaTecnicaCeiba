using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace EventosVivos.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Recurso no encontrado");
            await WriteError(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Regla de negocio violada");
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado");
            await WriteError(context, HttpStatusCode.InternalServerError, "Ha ocurrido un error interno.");
        }
    }

    private static async Task WriteError(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        var body = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(body);
    }
}
