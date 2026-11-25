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

public class RefreshTokenLN : BaseLN<RefreshTokenLN>, IRefreshTokenLN
{
    private readonly IAutorizacionLN _autorizacion;

    public RefreshTokenLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<RefreshTokenLN> logger,
        IAutorizacionLN autorizacion)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // usuarioId debe obtenerse de la identidad autenticada en la capa de API.
    public async Task<Respuesta<List<TRefreshToken>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TRefreshToken>>("No tenés permiso para consultar estos registros.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TRefreshToken>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<RefreshToken>().Consultar()
            .OrderBy(entidad => entidad.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TRefreshToken>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TRefreshToken>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<TRefreshToken>("No tenés permiso para consultar estos registros.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<RefreshToken>().Consultar()
            .Where(entidad => entidad.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TRefreshToken>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TRefreshToken>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<bool>> RevocarAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        bool administra = await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken);
        bool usuarioActivo = await _autorizacion.UsuarioActivoAsync(usuarioId, cancellationToken);
        if (!usuarioActivo)
        {
            return Fallar<bool>("La sesión no corresponde a un usuario activo.", 403);
        }

        var token = await _unidadDeTrabajo.Repositorio<RefreshToken>()
            .ObtenerEntidadAsync(registro => registro.Id == id, cancellationToken);
        if (token == null || (!administra && token.UsuarioId != usuarioId))
        {
            return Fallar<bool>("No existe la sesión solicitada.", 404);
        }

        if (token.FechaRevocacion == null)
        {
            token.FechaRevocacion = DateTime.UtcNow;
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        }

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static Expression<Func<RefreshToken, TRefreshToken>> Proyectar()
    {
        return entidad => new TRefreshToken
        {
            Id = entidad.Id,
            UsuarioId = entidad.UsuarioId,
            FechaCreacion = entidad.FechaCreacion,
            FechaExpiracion = entidad.FechaExpiracion,
            FechaRevocacion = entidad.FechaRevocacion,
            TokenReemplazoId = entidad.TokenReemplazoId
        };
    }
}
