using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class AjusteCuentaLN : BaseHospedajeLN<AjusteCuentaLN>, IAjusteCuentaLN
{
    private readonly IHospedajeAD _hospedaje;

    public AjusteCuentaLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<AjusteCuentaLN> logger, IHospedajeAD hospedaje)
        : base(unidad, mapper, logger)
    {
        _hospedaje = hospedaje;
    }

    public async Task<Respuesta<List<TAjusteCuenta>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TAjusteCuenta>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<TAjusteCuenta>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<AjusteCuenta>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new TAjusteCuenta
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                Monto = dato.Monto,
                Motivo = dato.Motivo,
                UsuarioId = dato.UsuarioId,
                AutorizadoPorId = dato.AutorizadoPorId,
                Fecha = dato.Fecha
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TAjusteCuenta>>
        {
            Data = resultados
        };
    }

public async Task<Respuesta<bool>> RegistrarAsync(
        int reservacionId,
        decimal monto,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 300 || monto == 0
            || monto < -99999999.99m || monto > 99999999.99m || decimal.Round(monto, 2) != monto)
        {
            return Fallar<bool>("Indicá una justificación y un monto válido distinto de cero.", 400);
        }

        if (!await TienePermisoAsync(usuarioId, "Ajustes.Autorizar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para autorizar ajustes.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = reservacionId,
            ["@Monto"] = monto,
            ["@Motivo"] = motivo.Trim(),
            ["@UsuarioId"] = usuarioId,
            ["@AutorizadoPorId"] = usuarioId
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.RegistrarAjuste, parametros, cancellationToken);
    }
}
