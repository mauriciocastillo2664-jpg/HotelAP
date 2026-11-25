using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ICargoAdicionalLN
{
    Task<Respuesta<List<TCargoAdicional>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> RegistrarAsync(int reservacionId, string concepto, decimal monto, int usuarioId, CancellationToken cancellationToken = default);
}
