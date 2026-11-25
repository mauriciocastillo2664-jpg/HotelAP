using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IAmenidadLN
{
    Task<Respuesta<List<TAmenidad>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TAmenidad>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TAmenidad>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TAmenidad>> InsertarAsync(
        TAmenidad datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TAmenidad>> ModificarAsync(
        TAmenidad datos,
        CancellationToken cancellationToken = default);
}
