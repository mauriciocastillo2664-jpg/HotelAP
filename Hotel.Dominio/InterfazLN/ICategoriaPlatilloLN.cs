using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ICategoriaPlatilloLN
{
    Task<Respuesta<List<TCategoriaPlatillo>>> ListarAsync(
        int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<List<TCategoriaPlatillo>>> BuscarAsync(
        string texto, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCategoriaPlatillo>> ObtenerAsync(int id, CancellationToken cancellationToken = default);

    Task<Respuesta<TCategoriaPlatillo>> InsertarAsync(TCategoriaPlatillo datos, CancellationToken cancellationToken = default);

    Task<Respuesta<TCategoriaPlatillo>> ModificarAsync(TCategoriaPlatillo datos, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
