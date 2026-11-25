using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IReportesLN
{
    Task<Respuesta<List<TVwEstadoCuenta>>> ConsultarEstadoCuentaAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwEstadoUsuario>>> ConsultarEstadoUsuarioAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwFacturaAbono>>> ConsultarFacturaAbonoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwFacturaConcepto>>> ConsultarFacturaConceptoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwHistorialCorreo>>> ConsultarHistorialCorreoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwIngresosPorMes>>> ConsultarIngresosPorMesAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwOcupacionActual>>> ConsultarOcupacionActualAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwPlatillosMasVendido>>> ConsultarPlatillosMasVendidoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwReservacionesDetalle>>> ConsultarReservacionesDetalleAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwTiemposPreparacion>>> ConsultarTiemposPreparacionAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TVwUsuarioPermiso>>> ConsultarUsuarioPermisoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default);
}
