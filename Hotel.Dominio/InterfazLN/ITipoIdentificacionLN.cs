using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface ITipoIdentificacionLN
{
    Task<Respuesta<List<TTipoIdentificacion>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TTipoIdentificacion>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoIdentificacion>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoIdentificacion>> InsertarAsync(
        TTipoIdentificacion datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TTipoIdentificacion>> ModificarAsync(
        TTipoIdentificacion datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
