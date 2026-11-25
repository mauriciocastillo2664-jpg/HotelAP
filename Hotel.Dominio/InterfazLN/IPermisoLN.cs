using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IPermisoLN
{
    Task<Respuesta<List<TPermiso>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TPermiso>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

}
