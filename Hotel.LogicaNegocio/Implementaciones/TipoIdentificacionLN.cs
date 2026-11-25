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

public class TipoIdentificacionLN : BaseLN<TipoIdentificacionLN>, ITipoIdentificacionLN
{
    public TipoIdentificacionLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<TipoIdentificacionLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TTipoIdentificacion>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TTipoIdentificacion>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TTipoIdentificacion>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            consulta = consulta.Where(registro => registro.Nombre.Contains(filtro));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Nombre)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TTipoIdentificacion>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTipoIdentificacion>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TTipoIdentificacion>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TTipoIdentificacion>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTipoIdentificacion>> InsertarAsync(
        TTipoIdentificacion datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTipoIdentificacion>(error, 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar()
            .AnyAsync(registro => registro.Nombre == datos.Nombre.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TTipoIdentificacion>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new TipoIdentificacion();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = true;
        await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó TipoIdentificacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TTipoIdentificacion>> ModificarAsync(
        TTipoIdentificacion datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTipoIdentificacion>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TTipoIdentificacion>("No existe el registro a modificar.", 404);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Nombre == datos.Nombre.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TTipoIdentificacion>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó TipoIdentificacion con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó TipoIdentificacion con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TTipoIdentificacion datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 50)
        {
            return "Nombre es obligatorio y admite hasta 50 caracteres.";
        }

        if (datos.Formato?.Length > 50)
        {
            return "Formato admite hasta 50 caracteres.";
        }

        return string.Empty;
    }

    private static Expression<Func<TipoIdentificacion, TTipoIdentificacion>> Proyectar()
    {
        return registro => new TTipoIdentificacion
        {
            Id = registro.Id,
            Nombre = registro.Nombre,
            Formato = registro.Formato,
            Activo = registro.Activo
        };
    }
}
