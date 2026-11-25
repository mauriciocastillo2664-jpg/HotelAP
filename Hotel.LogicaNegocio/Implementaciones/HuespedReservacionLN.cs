using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class HuespedReservacionLN : BaseHospedajeLN<HuespedReservacionLN>, IHuespedReservacionLN
{

    public HuespedReservacionLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<HuespedReservacionLN> logger)
        : base(unidad, mapper, logger)
    {

    }

    public async Task<Respuesta<List<THuespedReservacion>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<THuespedReservacion>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<THuespedReservacion>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<HuespedReservacion>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new THuespedReservacion
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                NombreCompleto = dato.NombreCompleto,
                TipoIdentificacionId = dato.TipoIdentificacionId,
                NumeroIdentificacion = dato.NumeroIdentificacion,
                EsMenor = dato.EsMenor,
                EsTitular = dato.EsTitular
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<THuespedReservacion>>
        {
            Data = resultados
        };
    }


}
