using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IUsuarioLN
{
    Task<Respuesta<List<TUsuario>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TUsuario>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CambiarEstadoAsync(
        int usuarioId,
        int usuarioDestinoId,
        int estadoId,
        CancellationToken cancellationToken = default);
    Task<Respuesta<bool>> AsignarRolAsync(
        int usuarioId, int usuarioDestinoId, int rolId, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> QuitarRolAsync(
        int usuarioId, int usuarioDestinoId, int rolId, CancellationToken cancellationToken = default);

}
