using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IAbonoLN
{
    Task<Respuesta<List<TAbono>>> ListarPorPagoAsync(
        int pagoId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TAbono>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> RegistrarAsync(
        TRegistrarAbono datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> ReversarAsync(
        TReversarAbono datos,
        int usuarioId,
        CancellationToken cancellationToken = default);
}
