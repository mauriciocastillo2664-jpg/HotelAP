using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IRolLN
{
    Task<Respuesta<List<TRol>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TRol>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> AsignarPermisoAsync(
        int usuarioId, int rolId, int permisoId, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> QuitarPermisoAsync(
        int usuarioId, int rolId, int permisoId, CancellationToken cancellationToken = default);

}
