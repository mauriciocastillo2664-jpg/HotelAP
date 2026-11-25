using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IFacturaDetalleLN
{
    Task<Respuesta<List<TFacturaDetalle>>> ListarPorFacturaAsync(
        int facturaId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TFacturaDetalle>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default);


}
