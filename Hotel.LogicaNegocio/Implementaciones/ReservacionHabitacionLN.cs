using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class ReservacionHabitacionLN : BaseHospedajeLN<ReservacionHabitacionLN>, IReservacionHabitacionLN
{

    public ReservacionHabitacionLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<ReservacionHabitacionLN> logger)
        : base(unidad, mapper, logger)
    {

    }

    public async Task<Respuesta<List<TReservacionHabitacion>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TReservacionHabitacion>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<TReservacionHabitacion>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<ReservacionHabitacion>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new TReservacionHabitacion
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                HabitacionId = dato.HabitacionId,
                FechaInicio = dato.FechaInicio,
                FechaFin = dato.FechaFin,
                Subtotal = dato.Subtotal
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TReservacionHabitacion>>
        {
            Data = resultados
        };
    }


}
