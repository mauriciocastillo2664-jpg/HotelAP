using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IPagoLN
{
    Task<Respuesta<List<TPago>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPago>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);


}
