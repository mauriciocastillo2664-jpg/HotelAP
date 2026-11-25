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

public class TokenUsuarioLN : BaseLN<TokenUsuarioLN>, ITokenUsuarioLN
{
    private readonly IAutorizacionLN _autorizacion;

    public TokenUsuarioLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<TokenUsuarioLN> logger,
        IAutorizacionLN autorizacion)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // usuarioId debe obtenerse de la identidad autenticada en la capa de API.
    public async Task<Respuesta<List<TTokenUsuario>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TTokenUsuario>>("No tenés permiso para consultar estos registros.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TTokenUsuario>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<TokenUsuario>().Consultar()
            .OrderBy(entidad => entidad.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TTokenUsuario>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTokenUsuario>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<TTokenUsuario>("No tenés permiso para consultar estos registros.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<TokenUsuario>().Consultar()
            .Where(entidad => entidad.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TTokenUsuario>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TTokenUsuario>
        {
            Data = datos
        };
    }

    private static Expression<Func<TokenUsuario, TTokenUsuario>> Proyectar()
    {
        return entidad => new TTokenUsuario
        {
            Id = entidad.Id,
            UsuarioId = entidad.UsuarioId,
            Tipo = entidad.Tipo,
            FechaCreacion = entidad.FechaCreacion,
            FechaExpiracion = entidad.FechaExpiracion,
            FechaUso = entidad.FechaUso
        };
    }
}
