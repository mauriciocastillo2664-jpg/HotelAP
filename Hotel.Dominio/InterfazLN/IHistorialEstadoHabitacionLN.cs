using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IHistorialEstadoHabitacionLN
{
    Task<Respuesta<List<THistorialEstadoHabitacion>>> ListarAsync(int habitacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);
}
