using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IFacturaLN
{
    Task<Respuesta<List<TFactura>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TFactura>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);


}
