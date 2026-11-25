using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IClienteLN
{
    Task<Respuesta<List<TCliente>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TCliente>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCliente>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCliente>> InsertarAsync(
        TCliente datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCliente>> ModificarAsync(
        TCliente datos,
        CancellationToken cancellationToken = default);
}
