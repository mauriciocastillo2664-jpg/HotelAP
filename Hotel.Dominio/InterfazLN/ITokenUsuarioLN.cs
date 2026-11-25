using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ITokenUsuarioLN
{
    Task<Respuesta<List<TTokenUsuario>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTokenUsuario>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

}
