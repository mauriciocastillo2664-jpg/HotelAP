using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfazLN;

public interface IParametroLN
{
    Task<Respuesta<List<TParametro>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TParametro>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TParametro>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TParametro>> InsertarAsync(
        TParametro datos,
        CancellationToken cancellationToken = default);

    Task<Respuesta<TParametro>> ModificarAsync(
        TParametro datos,
        CancellationToken cancellationToken = default);
}
