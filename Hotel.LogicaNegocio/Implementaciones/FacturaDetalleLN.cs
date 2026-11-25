using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class FacturaDetalleLN : BaseLN<FacturaDetalleLN>, IFacturaDetalleLN
{
    private readonly IAutorizacionLN _autorizacion;

    public FacturaDetalleLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<FacturaDetalleLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TFacturaDetalle>>> ListarPorFacturaAsync(
        int facturaId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (facturaId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TFacturaDetalle>>("Indicá un identificador positivo y una paginación válida.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<Factura>().Consultar()
            .Where(registro => registro.Id == facturaId)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TFacturaDetalle>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<FacturaDetalle>().Consultar()
            .Where(registro => registro.FacturaId == facturaId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TFacturaDetalle
            {
                Id = registro.Id,
                FacturaId = registro.FacturaId,
                TipoConcepto = registro.TipoConcepto,
                Descripcion = registro.Descripcion,
                Cantidad = registro.Cantidad,
                PrecioUnitario = registro.PrecioUnitario,
                Subtotal = registro.Subtotal
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TFacturaDetalle>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TFacturaDetalle>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TFacturaDetalle>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<FacturaDetalle>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.Factura.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TFacturaDetalle>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TFacturaDetalle>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<FacturaDetalle>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TFacturaDetalle
            {
                Id = registro.Id,
                FacturaId = registro.FacturaId,
                TipoConcepto = registro.TipoConcepto,
                Descripcion = registro.Descripcion,
                Cantidad = registro.Cantidad,
                PrecioUnitario = registro.PrecioUnitario,
                Subtotal = registro.Subtotal
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TFacturaDetalle>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TFacturaDetalle>
        {
            Data = datos
        };
    }


    private async Task<bool> PuedeConsultarClienteAsync(
        int? clienteId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        if (!await _autorizacion.UsuarioActivoAsync(usuarioId, cancellationToken))
        {
            return false;
        }

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Facturas.Ver", cancellationToken))
        {
            return true;
        }

        if (clienteId == null)
        {
            return false;
        }

        return await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId && usuario.ClienteId == clienteId, cancellationToken);
    }
}
