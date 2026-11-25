using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IMetodoPagoLN
{
    Task<Respuesta<List<TMetodoPago>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TMetodoPago>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TMetodoPago>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TMetodoPago>> InsertarAsync(
        TMetodoPago datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TMetodoPago>> ModificarAsync(
        TMetodoPago datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
