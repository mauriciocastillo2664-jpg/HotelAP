using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ICorreoLN
{
    Task<Respuesta<List<TCorreo>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCorreo>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TCorreo>> RegistrarAsync(
        TRegistrarCorreo datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> ActualizarEstadoAsync(
        TActualizarEstadoCorreo datos,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
