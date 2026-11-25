using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Hotel.LogicaNegocio.Implementaciones;

public class RolLN : BaseLN<RolLN>, IRolLN
{
    private readonly IAutorizacionLN _autorizacion;

    public RolLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<RolLN> logger,
        IAutorizacionLN autorizacion)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // usuarioId debe obtenerse de la identidad autenticada en la capa de API.
    public async Task<Respuesta<List<TRol>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TRol>>("No tenés permiso para consultar estos registros.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TRol>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Rol>().Consultar()
            .OrderBy(entidad => entidad.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TRol>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TRol>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<TRol>("No tenés permiso para consultar estos registros.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Rol>().Consultar()
            .Where(entidad => entidad.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TRol>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TRol>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<bool>> AsignarPermisoAsync(
        int usuarioId, int rolId, int permisoId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar usuarios y roles.", 403);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Rol>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Permisos)
            .SingleOrDefaultAsync(registro => registro.Id == rolId, cancellationToken);
        var relacionado = await _unidadDeTrabajo.Repositorio<Permiso>()
            .ObtenerEntidadAsync(registro => registro.Id == permisoId, cancellationToken);

        if (entidad == null || relacionado == null)
        {
            return Fallar<bool>("No existe el usuario, rol o permiso solicitado.", 404);
        }

        bool asignado = entidad.Permisos.Any(registro => registro.Id == permisoId);

        if (!asignado)
        {
            entidad.Permisos.Add(relacionado);
            var auditoria = new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "AsignarPermiso",
                Entidad = "Rol",
                EntidadId = entidad.Id,
                Resultado = "Exito",
                FechaUtc = DateTime.UtcNow
            };
            await _unidadDeTrabajo.Repositorio<Auditoria>().InsertarAsync(auditoria, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        }

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    public async Task<Respuesta<bool>> QuitarPermisoAsync(
        int usuarioId, int rolId, int permisoId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar usuarios y roles.", 403);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Rol>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Permisos)
            .SingleOrDefaultAsync(registro => registro.Id == rolId, cancellationToken);
        var relacionado = await _unidadDeTrabajo.Repositorio<Permiso>()
            .ObtenerEntidadAsync(registro => registro.Id == permisoId, cancellationToken);

        if (entidad == null || relacionado == null)
        {
            return Fallar<bool>("No existe el usuario, rol o permiso solicitado.", 404);
        }

        bool asignado = entidad.Permisos.Any(registro => registro.Id == permisoId);

        if (asignado)
        {
            entidad.Permisos.Remove(relacionado);
            var auditoria = new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "QuitarPermiso",
                Entidad = "Rol",
                EntidadId = entidad.Id,
                Resultado = "Exito",
                FechaUtc = DateTime.UtcNow
            };
            await _unidadDeTrabajo.Repositorio<Auditoria>().InsertarAsync(auditoria, cancellationToken);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        }

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static Expression<Func<Rol, TRol>> Proyectar()
    {
        return entidad => new TRol
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
            Descripcion = entidad.Descripcion
        };
    }
}
