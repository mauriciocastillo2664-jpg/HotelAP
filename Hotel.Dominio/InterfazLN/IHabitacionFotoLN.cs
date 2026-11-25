using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IHabitacionFotoLN
{
    Task<Respuesta<List<THabitacionFoto>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<THabitacionFoto>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacionFoto>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacionFoto>> InsertarAsync(
        THabitacionFoto datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacionFoto>> ModificarAsync(
        THabitacionFoto datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
