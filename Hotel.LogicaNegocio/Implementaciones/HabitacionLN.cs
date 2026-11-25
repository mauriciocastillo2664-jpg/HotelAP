using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Hotel.LogicaNegocio.Implementaciones;

public class HabitacionLN : BaseLN<HabitacionLN>, IHabitacionLN
{
    private readonly IAutorizacionLN _autorizacion;
    private readonly IEstadoOperativoAD _estadosOperativos;

    public HabitacionLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<HabitacionLN> logger,
        IAutorizacionLN autorizacion,
        IEstadoOperativoAD estadosOperativos)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
        _estadosOperativos = estadosOperativos;
    }

    public Task<Respuesta<List<THabitacion>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<THabitacion>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<THabitacion>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Habitacion>().Consultar();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            consulta = consulta.Where(registro => registro.Numero.Contains(filtro));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Numero)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<THabitacion>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<THabitacion>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<THabitacion>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<THabitacion>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<THabitacion>> InsertarAsync(
        THabitacion datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<THabitacion>(error, 400);
        }

        if (!await _unidadDeTrabajo.Repositorio<TipoHabitacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.TipoHabitacionId && registro.Activo, cancellationToken))
        {
            return Fallar<THabitacion>("La referencia TipoHabitacionId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .AnyAsync(registro => registro.Numero == datos.Numero.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<THabitacion>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Habitacion();

        _mapper.Map(datos, entidad);
        entidad.Numero = datos.Numero.Trim();
        entidad.Activo = true;
        entidad.EstadoHabitacionId = 1;
        await _unidadDeTrabajo.Repositorio<Habitacion>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Habitacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<THabitacion>> ModificarAsync(
        THabitacion datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<THabitacion>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Habitacion>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<THabitacion>("No existe el registro a modificar.", 404);
        }

        if (datos.RowVersion == null || !datos.RowVersion.SequenceEqual(entidad.RowVersion))
        {
            return Fallar<THabitacion>("El registro cambió. Actualizá los datos antes de modificarlo.", 409);
        }

        if (!await _unidadDeTrabajo.Repositorio<TipoHabitacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.TipoHabitacionId && registro.Activo, cancellationToken))
        {
            return Fallar<THabitacion>("La referencia TipoHabitacionId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Numero == datos.Numero.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<THabitacion>("Ya existe un registro con esos datos únicos.", 409);
        }

        bool tieneReservas = await _unidadDeTrabajo.Repositorio<ReservacionHabitacion>().Consultar()
            .AnyAsync(registro => registro.HabitacionId == datos.Id
                && registro.Reservacion.EstadoReservacion.OcupaHabitacion
                && registro.FechaFin > DateOnly.FromDateTime(DateTime.Today), cancellationToken);

        if (tieneReservas && (!datos.Activo || entidad.TipoHabitacionId != datos.TipoHabitacionId))
        {
            return Fallar<THabitacion>("No se puede desactivar ni cambiar el tipo de una habitación con reservaciones vigentes.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Numero = datos.Numero.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<Habitacion>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Habitacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<Habitacion>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        bool tieneReservas = await _unidadDeTrabajo.Repositorio<ReservacionHabitacion>().Consultar()
            .AnyAsync(registro => registro.HabitacionId == id
                && registro.Reservacion.EstadoReservacion.OcupaHabitacion
                && registro.FechaFin > DateOnly.FromDateTime(DateTime.Today), cancellationToken);

        if (tieneReservas)
        {
            return Fallar<bool>("La habitación tiene reservaciones vigentes.", 409);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<Habitacion>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó Habitacion con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    public async Task<Respuesta<List<TAmenidad>>> ListarAmenidadesAsync(
        int habitacionId,
        CancellationToken cancellationToken = default)
    {
        if (habitacionId < 1)
        {
            return Fallar<List<TAmenidad>>("El identificador de habitación debe ser positivo.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .AnyAsync(registro => registro.Id == habitacionId, cancellationToken);

        if (!existe)
        {
            return Fallar<List<TAmenidad>>("No existe la habitación solicitada.", 404);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .Where(registro => registro.Id == habitacionId)
            .SelectMany(registro => registro.Amenidads)
            .OrderBy(registro => registro.Nombre)
            .ThenBy(registro => registro.Id)
            .Select(registro => new TAmenidad
            {
                Id = registro.Id,
                Nombre = registro.Nombre
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TAmenidad>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<bool>> AsignarAmenidadAsync(
        int habitacionId,
        int amenidadId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Habitaciones.Editar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para modificar amenidades de habitaciones.", 403);
        }

        if (habitacionId < 1 || amenidadId < 1)
        {
            return Fallar<bool>("Los identificadores de habitación y amenidad deben ser positivos.", 400);
        }

        var habitacion = await _unidadDeTrabajo.Repositorio<Habitacion>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Amenidads)
            .SingleOrDefaultAsync(registro => registro.Id == habitacionId, cancellationToken);
        var amenidad = await _unidadDeTrabajo.Repositorio<Amenidad>()
            .ObtenerEntidadAsync(registro => registro.Id == amenidadId, cancellationToken);

        if (habitacion == null || amenidad == null)
        {
            return Fallar<bool>("No existe la habitación o la amenidad solicitada.", 404);
        }

        if (!habitacion.Activo)
        {
            return Fallar<bool>("La habitación está desactivada.", 409);
        }

        bool asignada = habitacion.Amenidads.Any(registro => registro.Id == amenidadId);

        if (!asignada)
        {
            habitacion.Amenidads.Add(amenidad);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
            _logger.LogInformation("Se asignó la amenidad {AmenidadId} a la habitación {HabitacionId}.", amenidadId, habitacionId);
        }

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    public async Task<Respuesta<bool>> QuitarAmenidadAsync(
        int habitacionId,
        int amenidadId,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Habitaciones.Editar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para modificar amenidades de habitaciones.", 403);
        }

        if (habitacionId < 1 || amenidadId < 1)
        {
            return Fallar<bool>("Los identificadores de habitación y amenidad deben ser positivos.", 400);
        }

        var habitacion = await _unidadDeTrabajo.Repositorio<Habitacion>()
            .Consultar(seguimiento: true)
            .Include(registro => registro.Amenidads)
            .SingleOrDefaultAsync(registro => registro.Id == habitacionId, cancellationToken);
        var amenidad = await _unidadDeTrabajo.Repositorio<Amenidad>()
            .ObtenerEntidadAsync(registro => registro.Id == amenidadId, cancellationToken);

        if (habitacion == null || amenidad == null)
        {
            return Fallar<bool>("No existe la habitación o la amenidad solicitada.", 404);
        }

        bool asignada = habitacion.Amenidads.Any(registro => registro.Id == amenidadId);

        if (asignada)
        {
            habitacion.Amenidads.Remove(amenidad);
            await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
            _logger.LogInformation("Se quitó la amenidad {AmenidadId} de la habitación {HabitacionId}.", amenidadId, habitacionId);
        }

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    // usuarioId debe obtenerse de la identidad autenticada en la API.
    public async Task<Respuesta<bool>> CambiarEstadoAsync(
        int habitacionId,
        int estadoId,
        string motivo,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Habitaciones.CambiarEstado", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para cambiar el estado de habitaciones.", 403);
        }

        if (habitacionId < 1 || estadoId < 1 || motivo?.Length > 200)
        {
            return Fallar<bool>("Revisá los identificadores y el motivo, que admite hasta 200 caracteres.", 400);
        }

        return await _estadosOperativos.CambiarEstadoHabitacionAsync(
            habitacionId, estadoId, motivo ?? string.Empty, usuarioId, cancellationToken);
    }

    private static string Validar(THabitacion datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Numero) || datos.Numero.Trim().Length > 10)
        {
            return "Numero es obligatorio y admite hasta 10 caracteres.";
        }

        if (datos.Piso < 0)
        {
            return "El piso debe ser no negativo.";
        }

        return string.Empty;
    }

    private static Expression<Func<Habitacion, THabitacion>> Proyectar()
    {
        return registro => new THabitacion
        {
            Id = registro.Id,
            Numero = registro.Numero,
            Piso = registro.Piso,
            TipoHabitacionId = registro.TipoHabitacionId,
            EstadoHabitacionId = registro.EstadoHabitacionId,
            Activo = registro.Activo,
            RowVersion = registro.RowVersion
        };
    }
}
