using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.InterfacesAD;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public abstract class BaseHospedajeLN<T> : BaseLN<T>
{
    protected BaseHospedajeLN(IUnidadTrabajoEF unidad, IMapper mapper, ILogger<T> logger)
        : base(unidad, logger, mapper)
    {
    }

    // usuarioId siempre debe provenir de la identidad autenticada, nunca del cuerpo HTTP.
    protected async Task<bool> TienePermisoAsync(
        int usuarioId,
        string permiso,
        CancellationToken cancellationToken)
    {
        return await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId
                && usuario.EstadoUsuario.PermiteAcceso
                && usuario.Rols.Any(rol => rol.Permisos.Any(dato => dato.Codigo == permiso)), cancellationToken);
    }

    protected async Task<bool> PuedeConsultarReservacionAsync(
        int reservacionId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        bool personal = await TienePermisoAsync(usuarioId, "Reservaciones.Leer", cancellationToken);
        if (personal)
        {
            return true;
        }

        return await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId
                && usuario.EstadoUsuario.PermiteAcceso
                && usuario.ClienteId != null
                && usuario.Cliente!.Reservaciones.Any(reserva => reserva.Id == reservacionId), cancellationToken);
    }

    protected static bool FechasValidas(DateOnly inicio, DateOnly fin, int adultos, int ninos)
    {
        return inicio < fin && adultos >= 1 && ninos >= 0;
    }
}
