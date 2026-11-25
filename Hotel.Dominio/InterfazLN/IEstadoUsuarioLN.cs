using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IEstadoUsuarioLN
{
    Task<Respuesta<List<TEstadoUsuario>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TEstadoUsuario>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TEstadoUsuario>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);
}
