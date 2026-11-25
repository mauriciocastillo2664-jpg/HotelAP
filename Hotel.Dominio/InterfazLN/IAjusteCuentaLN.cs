using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IAjusteCuentaLN
{
    Task<Respuesta<List<TAjusteCuenta>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> RegistrarAsync(int reservacionId, decimal monto, string motivo, int usuarioId, CancellationToken cancellationToken = default);
}
