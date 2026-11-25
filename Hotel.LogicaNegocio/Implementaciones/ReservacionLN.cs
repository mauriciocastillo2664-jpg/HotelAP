using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class ReservacionLN : BaseHospedajeLN<ReservacionLN>, IReservacionLN
{
    private readonly IHospedajeAD _hospedaje;

    public ReservacionLN(
        IUnidadTrabajoEF unidad,
        IMapper mapper,
        ILogger<ReservacionLN> logger, IHospedajeAD hospedaje)
        : base(unidad, mapper, logger)
    {
        _hospedaje = hospedaje;
    }

    public async Task<Respuesta<List<TReservacion>>> ListarAsync(int usuarioId, int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TReservacion>>("Revisá la página y el tamaño solicitado (1 a 100).", 400);
        }

        bool personal = await TienePermisoAsync(usuarioId, "Reservaciones.Leer", cancellationToken);
        var usuario = await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .Where(dato => dato.Id == usuarioId && dato.EstadoUsuario.PermiteAcceso)
            .Select(dato => new { dato.ClienteId })
            .SingleOrDefaultAsync(cancellationToken);
        if (usuario == null || (!personal && usuario.ClienteId == null))
        {
            return Fallar<List<TReservacion>>("No tenés permiso para consultar estas reservaciones.", 403);
        }

        var consulta = _unidadDeTrabajo.Repositorio<Reservacion>().Consultar();
        if (!personal)
        {
            consulta = consulta.Where(dato => dato.ClienteId == usuario!.ClienteId);
        }

        var resultados = await consulta
            .OrderBy(dato => dato.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(dato => new TReservacion
            {
                Id = dato.Id,
                Codigo = dato.Codigo,
                ClienteId = dato.ClienteId,
                FechaInicio = dato.FechaInicio,
                FechaFin = dato.FechaFin,
                CantidadAdultos = dato.CantidadAdultos,
                CantidadNinos = dato.CantidadNinos,
                EstadoReservacionId = dato.EstadoReservacionId,
                DescuentoId = dato.DescuentoId,
                Subtotal = dato.Subtotal,
                Descuento = dato.Descuento,
                Impuestos = dato.Impuestos,
                Total = dato.Total,
                AnticipoRequerido = dato.AnticipoRequerido,
                Observaciones = dato.Observaciones,
                MotivoCancelacion = dato.MotivoCancelacion,
                UsuarioCreacionId = dato.UsuarioCreacionId,
                FechaCreacion = dato.FechaCreacion,
                RowVersion = dato.RowVersion
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TReservacion>>
        {
            Data = resultados
        };
    }

    public async Task<Respuesta<TReservacion>> ObtenerAsync(int id, int usuarioId, CancellationToken cancellationToken = default)
    {
        if (!await PuedeConsultarReservacionAsync(id, usuarioId, cancellationToken))
        {
            return Fallar<TReservacion>("No tenés permiso para consultar esta reservación.", 403);
        }

        var resultado = await _unidadDeTrabajo.Repositorio<Reservacion>().Consultar()
            .Where(dato => dato.Id == id)
            .Select(dato => new TReservacion
            {
                Id = dato.Id,
                Codigo = dato.Codigo,
                ClienteId = dato.ClienteId,
                FechaInicio = dato.FechaInicio,
                FechaFin = dato.FechaFin,
                CantidadAdultos = dato.CantidadAdultos,
                CantidadNinos = dato.CantidadNinos,
                EstadoReservacionId = dato.EstadoReservacionId,
                DescuentoId = dato.DescuentoId,
                Subtotal = dato.Subtotal,
                Descuento = dato.Descuento,
                Impuestos = dato.Impuestos,
                Total = dato.Total,
                AnticipoRequerido = dato.AnticipoRequerido,
                Observaciones = dato.Observaciones,
                MotivoCancelacion = dato.MotivoCancelacion,
                UsuarioCreacionId = dato.UsuarioCreacionId,
                FechaCreacion = dato.FechaCreacion,
                RowVersion = dato.RowVersion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (resultado == null)
        {
            return Fallar<TReservacion>("No existe la reservación solicitada.", 404);
        }

        return new Respuesta<TReservacion>
        {
            Data = resultado
        };
    }

public async Task<Respuesta<TReservacion>> InsertarAsync(
        TCrearReservacion datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!FechasValidas(datos.FechaInicio, datos.FechaFin, datos.CantidadAdultos, datos.CantidadNinos)
            || datos.ClienteId < 1 || datos.HabitacionId < 1
            || datos.Observaciones?.Length > 300 || datos.CodigoDescuento?.Length > 20)
        {
            return Fallar<TReservacion>("Revisá las fechas, los huéspedes y los datos de la reservación.", 400);
        }

        bool personal = await TienePermisoAsync(usuarioId, "Reservaciones.Crear", cancellationToken);
        bool propietario = await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId && usuario.EstadoUsuario.PermiteAcceso
                && usuario.ClienteId == datos.ClienteId, cancellationToken);
        if (!personal && !propietario)
        {
            return Fallar<TReservacion>("No tenés permiso para crear esta reservación.", 403);
        }

        var resultado = await _hospedaje.CrearReservacionAsync(datos, usuarioId, cancellationToken);
        if (!resultado.Success)
        {
            return Fallar<TReservacion>(resultado.Error, resultado.EstadoHttp);
        }

        return await ObtenerAsync(resultado.Data, usuarioId, cancellationToken);
    }

    public async Task<Respuesta<bool>> ModificarAsync(
        TModificarReservacion datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!FechasValidas(datos.FechaInicio, datos.FechaFin, datos.CantidadAdultos, datos.CantidadNinos))
        {
            return Fallar<bool>("Revisá el intervalo de fechas y la cantidad de huéspedes.", 400);
        }

        int? clienteId = await ClienteAutorizadoAsync(datos.ReservacionId, usuarioId, "Reservaciones.Modificar", cancellationToken);
        if (clienteId == -1)
        {
            return Fallar<bool>("No tenés permiso para modificar esta reservación.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = datos.ReservacionId,
            ["@Inicio"] = datos.FechaInicio.ToDateTime(TimeOnly.MinValue),
            ["@Fin"] = datos.FechaFin.ToDateTime(TimeOnly.MinValue),
            ["@Adultos"] = datos.CantidadAdultos,
            ["@Ninos"] = datos.CantidadNinos,
            ["@UsuarioId"] = usuarioId,
            ["@ClienteId"] = clienteId
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.ModificarReservacion, parametros, cancellationToken);
    }

    public async Task<Respuesta<bool>> CancelarAsync(
        int reservacionId,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 300)
        {
            return Fallar<bool>("Indicá un motivo de cancelación de hasta 300 caracteres.", 400);
        }

        int? clienteId = await ClienteAutorizadoAsync(reservacionId, usuarioId, "Reservaciones.Cancelar", cancellationToken);
        if (clienteId == -1)
        {
            return Fallar<bool>("No tenés permiso para cancelar esta reservación.", 403);
        }

        var parametros = new Dictionary<string, object?>
        {
            ["@ReservacionId"] = reservacionId,
            ["@Motivo"] = motivo.Trim(),
            ["@UsuarioId"] = usuarioId,
            ["@ClienteId"] = clienteId
        };

        return await _hospedaje.EjecutarOperacionAsync(
            OperacionHospedaje.CancelarReservacion, parametros, cancellationToken);
    }

    public Task<Respuesta<List<TDisponibilidad>>> BuscarDisponibilidadAsync(
        TConsultaDisponibilidad datos,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina)
            || !FechasValidas(datos.FechaInicio, datos.FechaFin, datos.CantidadAdultos, datos.CantidadNinos))
        {
            return Task.FromResult(Fallar<List<TDisponibilidad>>("Revisá fechas, capacidad y paginación.", 400));
        }

        return _hospedaje.BuscarDisponibilidadAsync(datos, pagina, tamanoPagina, cancellationToken);
    }

    private async Task<int?> ClienteAutorizadoAsync(
        int reservacionId,
        int usuarioId,
        string permiso,
        CancellationToken cancellationToken)
    {
        if (await TienePermisoAsync(usuarioId, permiso, cancellationToken))
        {
            return null;
        }

        var usuario = await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .Where(dato => dato.Id == usuarioId && dato.EstadoUsuario.PermiteAcceso)
            .Select(dato => new { dato.ClienteId })
            .SingleOrDefaultAsync(cancellationToken);
        if (usuario?.ClienteId == null)
        {
            return -1;
        }

        bool propia = await _unidadDeTrabajo.Repositorio<Reservacion>().Consultar()
            .AnyAsync(dato => dato.Id == reservacionId && dato.ClienteId == usuario.ClienteId, cancellationToken);
        if (!propia)
        {
            return -1;
        }

        return usuario.ClienteId;
    }
}
