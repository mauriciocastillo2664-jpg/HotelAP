using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IPlatilloLN
{
    Task<Respuesta<List<TPlatillo>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TPlatillo>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPlatillo>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPlatillo>> InsertarAsync(
        TPlatillo datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPlatillo>> ModificarAsync(
        TPlatillo datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
