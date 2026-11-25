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

public class AuditoriaLN : BaseLN<AuditoriaLN>, IAuditoriaLN
{
    private readonly IAutorizacionLN _autorizacion;

    public AuditoriaLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<AuditoriaLN> logger,
        IAutorizacionLN autorizacion)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // usuarioId debe obtenerse de la identidad autenticada en la capa de API.
    public async Task<Respuesta<List<TAuditoria>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Auditoria.Ver", cancellationToken))
        {
            return Fallar<List<TAuditoria>>("No tenés permiso para consultar estos registros.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TAuditoria>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Auditoria>().Consultar()
            .OrderBy(entidad => entidad.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TAuditoria>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TAuditoria>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Auditoria.Ver", cancellationToken))
        {
            return Fallar<TAuditoria>("No tenés permiso para consultar estos registros.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Auditoria>().Consultar()
            .Where(entidad => entidad.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TAuditoria>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TAuditoria>
        {
            Data = datos
        };
    }

    private static Expression<Func<Auditoria, TAuditoria>> Proyectar()
    {
        return entidad => new TAuditoria
        {
            Id = entidad.Id,
            UsuarioId = entidad.UsuarioId,
            Accion = entidad.Accion,
            Entidad = entidad.Entidad,
            EntidadId = entidad.EntidadId,
            Resultado = entidad.Resultado,
            FechaUtc = entidad.FechaUtc
        };
    }
}
