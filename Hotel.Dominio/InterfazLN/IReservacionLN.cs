using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IReservacionLN
{
    Task<Respuesta<List<TReservacion>>> ListarAsync(int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<TReservacion>> ObtenerAsync(int id, int usuarioId, CancellationToken cancellationToken = default);

    Task<Respuesta<TReservacion>> InsertarAsync(TCrearReservacion datos, int usuarioId, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> ModificarAsync(TModificarReservacion datos, int usuarioId, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CancelarAsync(int reservacionId, string motivo, int usuarioId, CancellationToken cancellationToken = default);

    Task<Respuesta<List<TDisponibilidad>>> BuscarDisponibilidadAsync(TConsultaDisponibilidad datos, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);
}
