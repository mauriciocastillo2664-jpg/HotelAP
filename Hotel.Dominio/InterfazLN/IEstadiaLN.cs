using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IEstadiaLN
{
    Task<Respuesta<List<TEstadia>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CheckInAsync(int reservacionId, int usuarioId, bool autorizarAnticipado = false, CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> CheckOutAsync(int reservacionId, int usuarioId, CancellationToken cancellationToken = default);
}
