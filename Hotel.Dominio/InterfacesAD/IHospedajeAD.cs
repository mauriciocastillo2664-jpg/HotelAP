using Hotel.Dominio.EntidadesTipadas;
using Hotel.Utilitarios;

namespace Hotel.Dominio.InterfacesAD;

public interface IHospedajeAD
{
    Task<Respuesta<int>> CrearReservacionAsync(
        TCrearReservacion datos,
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<Respuesta<bool>> EjecutarOperacionAsync(
        OperacionHospedaje operacion,
        IReadOnlyDictionary<string, object?> parametros,
        CancellationToken cancellationToken = default);

    Task<Respuesta<List<TDisponibilidad>>> BuscarDisponibilidadAsync(
        TConsultaDisponibilidad datos,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default);
}
