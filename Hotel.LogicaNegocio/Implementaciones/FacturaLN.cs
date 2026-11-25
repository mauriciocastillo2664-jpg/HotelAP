using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class FacturaLN : BaseLN<FacturaLN>, IFacturaLN
{
    private readonly IAutorizacionLN _autorizacion;

    public FacturaLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<FacturaLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TFactura>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (clienteId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TFactura>>("Indicá un identificador positivo y una paginación válida.", 400);
        }



        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TFactura>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Factura>().Consultar()
            .Where(registro => registro.ClienteId == clienteId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TFactura
            {
                Id = registro.Id,
                NumeroFactura = registro.NumeroFactura,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                PedidoId = registro.PedidoId,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                TotalPagado = registro.TotalPagado,
                FechaEmision = registro.FechaEmision,
                UsuarioId = registro.UsuarioId
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TFactura>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TFactura>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TFactura>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<Factura>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TFactura>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TFactura>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Factura>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TFactura
            {
                Id = registro.Id,
                NumeroFactura = registro.NumeroFactura,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                PedidoId = registro.PedidoId,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                TotalPagado = registro.TotalPagado,
                FechaEmision = registro.FechaEmision,
                UsuarioId = registro.UsuarioId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TFactura>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TFactura>
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
