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

public class HabitacionFotoLN : BaseLN<HabitacionFotoLN>, IHabitacionFotoLN
{
    public HabitacionFotoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<HabitacionFotoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<THabitacionFoto>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<THabitacionFoto>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<THabitacionFoto>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<HabitacionFoto>().Consultar();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            consulta = consulta.Where(registro => registro.Url.Contains(filtro));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Url)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<THabitacionFoto>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<THabitacionFoto>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<HabitacionFoto>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<THabitacionFoto>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<THabitacionFoto>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<THabitacionFoto>> InsertarAsync(
        THabitacionFoto datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<THabitacionFoto>(error, 400);
        }

        if (!await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.HabitacionId && registro.Activo, cancellationToken))
        {
            return Fallar<THabitacionFoto>("La referencia HabitacionId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<HabitacionFoto>().Consultar()
            .AnyAsync(registro => datos.EsPrincipal && registro.HabitacionId == datos.HabitacionId && registro.EsPrincipal, cancellationToken);

        if (existe)
        {
            return Fallar<THabitacionFoto>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new HabitacionFoto();

        _mapper.Map(datos, entidad);
        entidad.Url = datos.Url.Trim();
        await _unidadDeTrabajo.Repositorio<HabitacionFoto>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó HabitacionFoto con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<THabitacionFoto>> ModificarAsync(
        THabitacionFoto datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<THabitacionFoto>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<HabitacionFoto>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<THabitacionFoto>("No existe el registro a modificar.", 404);
        }

        if (!await _unidadDeTrabajo.Repositorio<Habitacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.HabitacionId && registro.Activo, cancellationToken))
        {
            return Fallar<THabitacionFoto>("La referencia HabitacionId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<HabitacionFoto>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (datos.EsPrincipal && registro.HabitacionId == datos.HabitacionId && registro.EsPrincipal), cancellationToken);

        if (existe)
        {
            return Fallar<THabitacionFoto>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Url = datos.Url.Trim();
        _unidadDeTrabajo.Repositorio<HabitacionFoto>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó HabitacionFoto con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<HabitacionFoto>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe la fotografía solicitada.", 404);
        }

        _unidadDeTrabajo.Repositorio<HabitacionFoto>().Eliminar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se eliminó la fotografía {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(THabitacionFoto datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Url) || datos.Url.Trim().Length > 300)
        {
            return "Url es obligatorio y admite hasta 300 caracteres.";
        }

        if (datos.Descripcion?.Length > 100)
        {
            return "Descripcion admite hasta 100 caracteres.";
        }

        if (datos.Orden < 0)
        {
            return "El orden debe ser no negativo.";
        }

        if (!Uri.TryCreate(datos.Url, UriKind.Absolute, out var imagen) || (imagen.Scheme != Uri.UriSchemeHttps && imagen.Scheme != Uri.UriSchemeHttp))
        {
            return "La imagen debe tener una dirección HTTP o HTTPS válida.";
        }

        return string.Empty;
    }

    private static Expression<Func<HabitacionFoto, THabitacionFoto>> Proyectar()
    {
        return registro => new THabitacionFoto
        {
            Id = registro.Id,
            HabitacionId = registro.HabitacionId,
            Url = registro.Url,
            Descripcion = registro.Descripcion,
            EsPrincipal = registro.EsPrincipal,
            Orden = registro.Orden
        };
    }
}
