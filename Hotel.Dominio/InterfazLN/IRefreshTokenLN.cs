using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IRefreshTokenLN
{
    Task<Respuesta<List<TRefreshToken>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TRefreshToken>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> RevocarAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

}
