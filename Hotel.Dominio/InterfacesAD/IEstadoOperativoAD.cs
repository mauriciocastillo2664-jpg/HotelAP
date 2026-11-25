using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfacesAD;

public interface IEstadoOperativoAD
{
    Task<Respuesta<bool>> CambiarEstadoUsuarioAsync(
        int usuarioDestinoId,
        int estadoId,
        int responsableId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CambiarEstadoHabitacionAsync(
        int habitacionId,
        int estadoId,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
