using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IHistorialEstadoPedidoLN
{
    Task<Respuesta<List<THistorialEstadoPedido>>> ListarPorPedidoAsync(
        int pedidoId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THistorialEstadoPedido>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);


}
