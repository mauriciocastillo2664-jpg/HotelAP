using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IHabitacionLN
{
    Task<Respuesta<List<THabitacion>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<THabitacion>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacion>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacion>> InsertarAsync(
        THabitacion datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<THabitacion>> ModificarAsync(
        THabitacion datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TAmenidad>>> ListarAmenidadesAsync(
        int habitacionId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> AsignarAmenidadAsync(
        int habitacionId,
        int amenidadId,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> QuitarAmenidadAsync(
        int habitacionId,
        int amenidadId,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CambiarEstadoAsync(
        int habitacionId,
        int estadoId,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
