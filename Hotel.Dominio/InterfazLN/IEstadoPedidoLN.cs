using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IEstadoPedidoLN
{
    Task<Respuesta<List<TEstadoPedido>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TEstadoPedido>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TEstadoPedido>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);
}
