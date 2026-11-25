using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class HistorialReservacionLN : BaseHospedajeLN<HistorialReservacionLN>, IHistorialReservacionLN
{

    public HistorialReservacionLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<HistorialReservacionLN> logger)
        : base(unidad, mapper, logger)
    {

    }

    public async Task<Respuesta<List<THistorialReservacion>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<THistorialReservacion>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<THistorialReservacion>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<HistorialReservacion>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new THistorialReservacion
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                EstadoAnteriorId = dato.EstadoAnteriorId,
                EstadoNuevoId = dato.EstadoNuevoId,
                UsuarioId = dato.UsuarioId,
                Motivo = dato.Motivo,
                Fecha = dato.Fecha
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<THistorialReservacion>>
        {
            Data = resultados
        };
    }


}
