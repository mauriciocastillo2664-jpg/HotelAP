using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IAuditoriaLN
{
    Task<Respuesta<List<TAuditoria>>> ListarAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TAuditoria>> ObtenerAsync(
        int usuarioId, int id, CancellationToken cancellationToken = default);

}
