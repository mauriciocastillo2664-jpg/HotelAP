using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfacesAD;

public interface IRestauranteFinanzasAD
{
    Task<Respuesta<int>> CrearPedidoAsync(
        TCrearPedido datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> AgregarPlatilloPedidoAsync(
        TAgregarPlatilloPedido datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CambiarEstadoPedidoAsync(
        TCambiarEstadoPedido datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<int>> PagarPedidoAsync(
        TPagarPedido datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> RegistrarAbonoAsync(
        TRegistrarAbono datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> ReversarAbonoAsync(
        TReversarAbono datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<int>> RegistrarCorreoAsync(
        TRegistrarCorreo datos, int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> ActualizarEstadoCorreoAsync(
        TActualizarEstadoCorreo datos,
        CancellationToken cancellationToken = default);

}
