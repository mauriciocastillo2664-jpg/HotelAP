using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ITipoHabitacionLN
{
    Task<Respuesta<List<TTipoHabitacion>>> ListarAsync(
        int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<List<TTipoHabitacion>>> BuscarAsync(
        string texto, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoHabitacion>> ObtenerAsync(int id, CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoHabitacion>> InsertarAsync(TTipoHabitacion datos, CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoHabitacion>> ModificarAsync(TTipoHabitacion datos, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
