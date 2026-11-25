using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class CargoAdicionalLN : BaseHospedajeLN<CargoAdicionalLN>, ICargoAdicionalLN
{
    private readonly IHospedajeAD _hospedaje;

    public CargoAdicionalLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<CargoAdicionalLN> logger, IHospedajeAD hospedaje)
        : base(unidad, mapper, logger)
    {
        _hospedaje = hospedaje;
    }

    public async Task<Respuesta<List<TCargoAdicional>>> ListarAsync(int reservacionId, int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TCargoAdicional>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        if (!await PuedeConsultarReservacionAsync(reservacionId, usuarioId, cancellationToken))
        {
            return Fallar<List<TCargoAdicional>>("No tenés permiso para consultar esta reservación.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<CargoAdicional>().Consultar()
            .Where(dato => dato.ReservacionId == reservacionId);

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new TCargoAdicional
            {
                Id = dato.Id,
                ReservacionId = dato.ReservacionId,
                Concepto = dato.Concepto,
                Monto = dato.Monto,
                Impuesto = dato.Impuesto,
                EsLateCheckout = dato.EsLateCheckout,
                UsuarioId = dato.UsuarioId,
                Fecha = dato.Fecha
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TCargoAdicional>>
        {
            Data = resultados
        };
    }

public async Task<Respuesta<bool>> RegistrarAsync(
        int reservacionId,
        string concepto,
        decimal monto,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(concepto) || concepto.Length > 150
            || monto <= 0 || monto > 99999999.99m || decimal.Round(monto, 2) != monto)
        {
            return Fallar<bool>("Indicá un concepto y un monto positivo con hasta dos decimales.", 400);
        }

        if (!await TienePermisoAsync(usuarioId, "Cargos.Registrar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para registrar cargos.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = reservacionId,
            ["@Concepto"] = concepto.Trim(),
            ["@Monto"] = monto,
            ["@UsuarioId"] = usuarioId
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.RegistrarCargoAdicional, parametros, cancellationToken);
    }
}
