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

public class MetodoPagoLN : BaseLN<MetodoPagoLN>, IMetodoPagoLN
{
    public MetodoPagoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<MetodoPagoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TMetodoPago>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TMetodoPago>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TMetodoPago>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<MetodoPago>().Consultar();

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

        return new Respuesta<List<TMetodoPago>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TMetodoPago>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<MetodoPago>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TMetodoPago>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TMetodoPago>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TMetodoPago>> InsertarAsync(
        TMetodoPago datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TMetodoPago>(error, 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<MetodoPago>().Consultar()
            .AnyAsync(registro => registro.Nombre == datos.Nombre.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TMetodoPago>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new MetodoPago();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = true;
        await _unidadDeTrabajo.Repositorio<MetodoPago>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó MetodoPago con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TMetodoPago>> ModificarAsync(
        TMetodoPago datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TMetodoPago>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<MetodoPago>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TMetodoPago>("No existe el registro a modificar.", 404);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<MetodoPago>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Nombre == datos.Nombre.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TMetodoPago>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<MetodoPago>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó MetodoPago con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<MetodoPago>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<MetodoPago>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó MetodoPago con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TMetodoPago datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 30)
        {
            return "Nombre es obligatorio y admite hasta 30 caracteres.";
        }

        return string.Empty;
    }

    private static Expression<Func<MetodoPago, TMetodoPago>> Proyectar()
    {
        return registro => new TMetodoPago
        {
            Id = registro.Id,
            Nombre = registro.Nombre,
            RequiereReferencia = registro.RequiereReferencia,
            Activo = registro.Activo
        };
    }
}
