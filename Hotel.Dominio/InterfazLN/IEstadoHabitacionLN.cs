using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IEstadoHabitacionLN
{
    Task<Respuesta<List<TEstadoHabitacion>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TEstadoHabitacion>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TEstadoHabitacion>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);
}
