using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hotel.API.Middleware;

public class ExcepcionesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExcepcionesMiddleware> _logger;

    public ExcepcionesMiddleware(
        RequestDelegate next,
        ILogger<ExcepcionesMiddleware> logger)
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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // La solicitud terminó porque el cliente cerró la conexión.
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Conflicto de concurrencia en {Ruta}.", context.Request.Path);
            await EscribirProblemaAsync(context, StatusCodes.Status409Conflict,
                "El registro cambió durante la operación. Actualizá los datos e intentá nuevamente.");
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _logger.LogWarning(exception, "Registro duplicado en {Ruta}.", context.Request.Path);
            await EscribirProblemaAsync(context, StatusCodes.Status409Conflict,
                "Ya existe un registro con esos datos únicos.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error no controlado en {Ruta}.", context.Request.Path);
            await EscribirProblemaAsync(context, StatusCodes.Status500InternalServerError,
                "No fue posible completar la operación.");
        }
    }

    private static async Task EscribirProblemaAsync(
        HttpContext context,
        int estado,
        string detalle)
    {
        if (context.Response.HasStarted)
        {
            context.Abort();
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = estado;
        var problema = new ProblemDetails
        {
            Status = estado,
            Title = estado == StatusCodes.Status409Conflict
                ? "Conflicto al guardar los datos"
                : "Error del servidor",
            Detail = detalle,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(
            problema,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }
}
