using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IPedidoRestauranteLN
{
    Task<Respuesta<List<TPedidoRestaurante>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPedidoRestaurante>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPedidoRestaurante>> CrearAsync(
        TCrearPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> AgregarPlatilloAsync(
        TAgregarPlatilloPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CambiarEstadoAsync(
        TCambiarEstadoPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TFactura>> PagarAsync(
        TPagarPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
