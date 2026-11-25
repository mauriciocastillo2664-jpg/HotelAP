using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IHistorialReservacionLN
{
    Task<Respuesta<List<THistorialReservacion>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);
}
