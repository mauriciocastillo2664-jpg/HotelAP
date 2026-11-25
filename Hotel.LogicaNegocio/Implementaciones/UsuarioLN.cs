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

public class UsuarioLN : BaseLN<UsuarioLN>, IUsuarioLN
{
    private readonly IAutorizacionLN _autorizacion;
    private readonly IEstadoOperativoAD _estados;

    public UsuarioLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<UsuarioLN> logger,
        IAutorizacionLN autorizacion, IEstadoOperativoAD estados)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
        _estados = estados;
    }

    // usuarioId debe obtenerse de la identidad autenticada en la capa de API.
    public async Task<Respuesta<List<TUsuario>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TUsuario>>("No tenés permiso para consultar estos registros.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TUsuario>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .OrderBy(entidad => entidad.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TUsuario>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TUsuario>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<TUsuario>("No tenés permiso para consultar estos registros.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .Where(entidad => entidad.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TUsuario>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TUsuario>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<bool>> CambiarEstadoAsync(
        int usuarioId,
        int usuarioDestinoId,
        int estadoId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar usuarios.", 403);
        }

        if (usuarioDestinoId <= 0 || estadoId <= 0)
        {
            return Fallar<bool>("El usuario y el estado deben ser positivos.", 400);
        }

        return await _estados.CambiarEstadoUsuarioAsync(
            usuarioDestinoId, estadoId, usuarioId, cancellationToken);
    }
    public async Task<Respuesta<bool>> AsignarRolAsync(
        int usuarioId, int usuarioDestinoId, int rolId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar usuarios y roles.", 403);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Usuario>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Rols)
            .SingleOrDefaultAsync(registro => registro.Id == usuarioDestinoId, cancellationToken);
        var relacionado = await _unidadDeTrabajo.Repositorio<Rol>()
            .ObtenerEntidadAsync(registro => registro.Id == rolId, cancellationToken);

        if (entidad == null || relacionado == null)
        {
            return Fallar<bool>("No existe el usuario, rol o permiso solicitado.", 404);
        }

        bool asignado = entidad.Rols.Any(registro => registro.Id == rolId);

        if (!asignado)
        {
            entidad.Rols.Add(relacionado);
            var auditoria = new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "AsignarRol",
                Entidad = "Usuario",
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

    public async Task<Respuesta<bool>> QuitarRolAsync(
        int usuarioId, int usuarioDestinoId, int rolId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar usuarios y roles.", 403);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Usuario>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Rols)
            .SingleOrDefaultAsync(registro => registro.Id == usuarioDestinoId, cancellationToken);
        var relacionado = await _unidadDeTrabajo.Repositorio<Rol>()
            .ObtenerEntidadAsync(registro => registro.Id == rolId, cancellationToken);

        if (entidad == null || relacionado == null)
        {
            return Fallar<bool>("No existe el usuario, rol o permiso solicitado.", 404);
        }

        bool asignado = entidad.Rols.Any(registro => registro.Id == rolId);

        if (asignado)
        {
            entidad.Rols.Remove(relacionado);
            var auditoria = new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "QuitarRol",
                Entidad = "Usuario",
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

    private static Expression<Func<Usuario, TUsuario>> Proyectar()
    {
        return entidad => new TUsuario
        {
            Id = entidad.Id,
            TipoIdentificacionId = entidad.TipoIdentificacionId,
            NumeroIdentificacion = entidad.NumeroIdentificacion,
            NombreUsuario = entidad.NombreUsuario,
            Email = entidad.Email,
            Nombres = entidad.Nombres,
            Apellidos = entidad.Apellidos,
            Telefono = entidad.Telefono,
            EstadoUsuarioId = entidad.EstadoUsuarioId,
            IntentosFallidos = entidad.IntentosFallidos,
            UltimoAcceso = entidad.UltimoAcceso,
            ClienteId = entidad.ClienteId,
            FechaCreacion = entidad.FechaCreacion,
            RowVersion = entidad.RowVersion
        };
    }
}
