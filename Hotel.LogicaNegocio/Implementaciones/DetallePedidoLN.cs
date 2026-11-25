using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class DetallePedidoLN : BaseLN<DetallePedidoLN>, IDetallePedidoLN
{
    private readonly IAutorizacionLN _autorizacion;

    public DetallePedidoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<DetallePedidoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TDetallePedido>>> ListarPorPedidoAsync(
        int pedidoId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (pedidoId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TDetallePedido>>("Indicá un identificador positivo y una paginación válida.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<PedidoRestaurante>().Consultar()
            .Where(registro => registro.Id == pedidoId)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TDetallePedido>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<DetallePedido>().Consultar()
            .Where(registro => registro.PedidoId == pedidoId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TDetallePedido
            {
                Id = registro.Id,
                PedidoId = registro.PedidoId,
                PlatilloId = registro.PlatilloId,
                Cantidad = registro.Cantidad,
                PrecioUnitario = registro.PrecioUnitario,
                Subtotal = registro.Subtotal
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TDetallePedido>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TDetallePedido>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TDetallePedido>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<DetallePedido>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.Pedido.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TDetallePedido>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TDetallePedido>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<DetallePedido>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TDetallePedido
            {
                Id = registro.Id,
                PedidoId = registro.PedidoId,
                PlatilloId = registro.PlatilloId,
                Cantidad = registro.Cantidad,
                PrecioUnitario = registro.PrecioUnitario,
                Subtotal = registro.Subtotal
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TDetallePedido>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TDetallePedido>
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

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Gestionar", cancellationToken))
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
