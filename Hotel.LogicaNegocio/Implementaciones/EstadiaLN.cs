using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class EstadiaLN : BaseHospedajeLN<EstadiaLN>, IEstadiaLN
{
    private readonly IHospedajeAD _hospedaje;

    public EstadiaLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<EstadiaLN> logger, IHospedajeAD hospedaje)
        : base(unidad, mapper, logger)
    {
        _hospedaje = hospedaje;
    }

    public async Task<Respuesta<List<TEstadia>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TEstadia>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<TEstadia>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<Estadia>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new TEstadia
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                CheckInReal = dato.CheckInReal,
                CheckOutReal = dato.CheckOutReal,
                UsuarioCheckInId = dato.UsuarioCheckInId,
                UsuarioCheckOutId = dato.UsuarioCheckOutId,
                CodigoAcceso = dato.CodigoAcceso
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TEstadia>>
        {
            Data = resultados
        };
    }

public async Task<Respuesta<bool>> CheckInAsync(
        int reservacionId,
        int usuarioId,
        bool autorizarAnticipado = false,
        CancellationToken cancellationToken = default)
    {
        if (!await TienePermisoAsync(usuarioId, "Estadias.CheckIn", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para registrar el check-in.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = reservacionId,
            ["@UsuarioId"] = usuarioId,
            ["@AutorizarAnticipado"] = autorizarAnticipado
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.CheckIn, parametros, cancellationToken);
    }

    public async Task<Respuesta<bool>> CheckOutAsync(
        int reservacionId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!await TienePermisoAsync(usuarioId, "Estadias.CheckOut", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para registrar el check-out.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = reservacionId,
            ["@UsuarioId"] = usuarioId
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.CheckOut, parametros, cancellationToken);
    }
}
