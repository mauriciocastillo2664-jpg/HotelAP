using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ITemporadaLN
{
    Task<Respuesta<List<TTemporada>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TTemporada>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTemporada>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTemporada>> InsertarAsync(
        TTemporada datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTemporada>> ModificarAsync(
        TTemporada datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
