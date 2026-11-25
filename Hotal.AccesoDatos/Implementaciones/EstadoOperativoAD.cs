using Hotel.AccesoDatos.Contexto;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.Entidades;
using Hotel.Utilitarios;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hotel.AccesoDatos.Implementaciones;

public class EstadoOperativoAD : IEstadoOperativoAD
{
    private readonly HotelContext _contexto;

    public EstadoOperativoAD(HotelContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<Respuesta<bool>> CambiarEstadoUsuarioAsync(
        int usuarioDestinoId,
        int estadoId,
        int responsableId,
        CancellationToken cancellationToken = default)
    {
        VerificarCambiosPendientes();

        try
        {
            await _contexto.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.sp_CambiarEstadoUsuario @UsuarioId={usuarioDestinoId}, @EstadoUsuarioId={estadoId}, @ResponsableId={responsableId}",
                cancellationToken);

            foreach (var entrada in _contexto.ChangeTracker.Entries<Usuario>()
                .Where(entrada => entrada.Entity.Id == usuarioDestinoId).ToList())
            {
                await entrada.ReloadAsync(cancellationToken);
            }

            foreach (var entrada in _contexto.ChangeTracker.Entries<RefreshToken>()
                .Where(entrada => entrada.Entity.UsuarioId == usuarioDestinoId).ToList())
            {
                await entrada.ReloadAsync(cancellationToken);
            }
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is 50001 or 50002 or 50004)
        {
            return CrearError(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> CambiarEstadoHabitacionAsync(
        int habitacionId,
        int estadoId,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        VerificarCambiosPendientes();

        try
        {
            await _contexto.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.sp_CambiarEstadoHabitacion @HabitacionId={habitacionId}, @EstadoHabitacionId={estadoId}, @Motivo={motivo}, @UsuarioId={usuarioId}",
                cancellationToken);

            foreach (var entrada in _contexto.ChangeTracker.Entries<Habitacion>()
                .Where(entrada => entrada.Entity.Id == habitacionId).ToList())
            {
                await entrada.ReloadAsync(cancellationToken);
            }
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is 50001 or 50002 or 50004)
        {
            return CrearError(exception.Number);
        }
    }

    private void VerificarCambiosPendientes()
    {
        if (_contexto.ChangeTracker.HasChanges())
        {
            throw new InvalidOperationException(
                "Guardá los cambios pendientes antes de ejecutar un cambio de estado.");
        }
    }
    private static Respuesta<bool> CrearError(int numero)
    {
        int estado = 400;
        string mensaje = "El cambio de estado no cumple las reglas de operación.";
        if (numero == 50004)
        {
            estado = 404;
            mensaje = "No existe el registro solicitado.";
        }
        else if (numero == 50002)
        {
            estado = 409;
            mensaje = "El estado actual impide completar la operación.";
        }

        return new Respuesta<bool>
        {
            Success = false,
            EstadoHttp = estado,
            Error = mensaje
        };
    }
}
