using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class HistorialEstadoHabitacionLN : BaseHospedajeLN<HistorialEstadoHabitacionLN>, IHistorialEstadoHabitacionLN
{

    public HistorialEstadoHabitacionLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<HistorialEstadoHabitacionLN> logger)
        : base(unidad, mapper, logger)
    {

    }

    public async Task<Respuesta<List<THistorialEstadoHabitacion>>> ListarAsync(int habitacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<THistorialEstadoHabitacion>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await TienePermisoAsync(usuarioId, "Habitaciones.Leer", cancellationToken))
        {
            return Fallar<List<THistorialEstadoHabitacion>>("No tenés permiso para consultar el historial.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<HistorialEstadoHabitacion>().Consultar()
            .Where(dato => dato.HabitacionId == habitacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new THistorialEstadoHabitacion
            {
                Id = dato.Id,
                HabitacionId = dato.HabitacionId,
                EstadoAnteriorId = dato.EstadoAnteriorId,
                EstadoNuevoId = dato.EstadoNuevoId,
                UsuarioId = dato.UsuarioId,
                Motivo = dato.Motivo,
                Fecha = dato.Fecha
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<THistorialEstadoHabitacion>>
        {
            Data = resultados
        };
    }


}
