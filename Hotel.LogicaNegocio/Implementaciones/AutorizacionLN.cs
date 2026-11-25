using Hotel.Dominio.Entidades;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Microsoft.EntityFrameworkCore;

namespace Hotel.LogicaNegocio.Implementaciones;

public class AutorizacionLN : IAutorizacionLN
{
    private readonly IUnidadTrabajoEF _unidadDeTrabajo;

    public AutorizacionLN(IUnidadTrabajoEF unidadDeTrabajo)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public Task<bool> UsuarioActivoAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId && usuario.EstadoUsuario.PermiteAcceso,
                cancellationToken);
    }

    public Task<bool> TienePermisoAsync(
        int usuarioId, string permiso, CancellationToken cancellationToken = default)
    {
        return _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId
                && usuario.EstadoUsuario.PermiteAcceso
                && usuario.Rols.Any(rol => rol.Permisos.Any(dato => dato.Codigo == permiso)),
                cancellationToken);
    }
}
