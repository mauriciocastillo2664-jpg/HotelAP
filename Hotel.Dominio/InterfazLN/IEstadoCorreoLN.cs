using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IEstadoCorreoLN
{
    Task<Respuesta<List<TEstadoCorreo>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TEstadoCorreo>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TEstadoCorreo>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);
}
