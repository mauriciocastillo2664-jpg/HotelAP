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

public class TipoHabitacionLN : BaseLN<TipoHabitacionLN>, ITipoHabitacionLN
{
    public TipoHabitacionLN(IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<TipoHabitacionLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TTipoHabitacion>>> ListarAsync(
        int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TTipoHabitacion>>> BuscarAsync(
        string texto, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TTipoHabitacion>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string nombre = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.TTipoHabitacion.Consultar();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            consulta = consulta.Where(registro => registro.Nombre.Contains(nombre));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Nombre)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TTipoHabitacion>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTipoHabitacion>> ObtenerAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.TTipoHabitacion.Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TTipoHabitacion>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TTipoHabitacion>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTipoHabitacion>> InsertarAsync(
        TTipoHabitacion datos, CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTipoHabitacion>(error, 400);
        }

        string nombre = datos.Nombre.Trim();
        bool existe = await _unidadDeTrabajo.TTipoHabitacion.Consultar()
            .AnyAsync(registro => registro.Nombre == nombre, cancellationToken);

        if (existe)
        {
            return Fallar<TTipoHabitacion>("Ya existe un registro con ese nombre.", 409);
        }

        var entidad = new TipoHabitacion();
        _mapper.Map(datos, entidad);
        entidad.Activo = true;

        await _unidadDeTrabajo.TTipoHabitacion.InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se creó TipoHabitacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TTipoHabitacion>> ModificarAsync(
        TTipoHabitacion datos, CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTipoHabitacion>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TTipoHabitacion>("No existe el registro a modificar.", 404);
        }

        string nombre = datos.Nombre.Trim();
        bool existe = await _unidadDeTrabajo.TTipoHabitacion.Consultar()
            .AnyAsync(registro => registro.Nombre == nombre && registro.Id != datos.Id, cancellationToken);

        if (existe)
        {
            return Fallar<TTipoHabitacion>("Ya existe otro registro con ese nombre.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.TTipoHabitacion.Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se modificó TipoHabitacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.TTipoHabitacion.ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.TTipoHabitacion.Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó TipoHabitacion con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TTipoHabitacion datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 50)
        {
            return "El nombre es obligatorio y admite hasta 50 caracteres.";
        }

        if (datos.Descripcion?.Length > 300)
        {
            return "La descripción admite hasta 300 caracteres.";
        }

        if (datos.CapacidadAdultos < 1 || datos.CapacidadNinos < 0)
        {
            return "Indicá al menos un adulto y una capacidad de niños no negativa.";
        }

        long capacidadTotal = (long)datos.CapacidadAdultos + datos.CapacidadNinos;
        if (datos.PersonasIncluidas < 1 || datos.PersonasIncluidas > capacidadTotal)
        {
            return "Las personas incluidas deben estar entre uno y la capacidad total.";
        }

        if (datos.PrecioBasePorNoche <= 0 || datos.PrecioBasePorNoche > 99999999.99m
            || decimal.Round(datos.PrecioBasePorNoche, 2) != datos.PrecioBasePorNoche)
        {
            return "El precio por noche debe ser positivo y tener hasta dos decimales.";
        }

        if (datos.CargoPersonaAdicional < 0 || datos.CargoPersonaAdicional > 99999999.99m
            || decimal.Round(datos.CargoPersonaAdicional, 2) != datos.CargoPersonaAdicional)
        {
            return "El cargo adicional debe ser no negativo y tener hasta dos decimales.";
        }

        return string.Empty;
    }

    private static Expression<Func<TipoHabitacion, TTipoHabitacion>> Proyectar()
    {
        return registro => new TTipoHabitacion
        {
            Id = registro.Id,
            Nombre = registro.Nombre,
            Descripcion = registro.Descripcion,
            CapacidadAdultos = registro.CapacidadAdultos,
            CapacidadNinos = registro.CapacidadNinos,
            PrecioBasePorNoche = registro.PrecioBasePorNoche,
            PersonasIncluidas = registro.PersonasIncluidas,
            CargoPersonaAdicional = registro.CargoPersonaAdicional,
            Activo = registro.Activo
        };
    }
}
