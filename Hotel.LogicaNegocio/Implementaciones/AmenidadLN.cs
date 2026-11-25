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

public class AmenidadLN : BaseLN<AmenidadLN>, IAmenidadLN
{
    public AmenidadLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<AmenidadLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TAmenidad>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TAmenidad>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TAmenidad>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Amenidad>().Consultar();

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

        return new Respuesta<List<TAmenidad>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TAmenidad>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Amenidad>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TAmenidad>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TAmenidad>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TAmenidad>> InsertarAsync(
        TAmenidad datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TAmenidad>(error, 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Amenidad>().Consultar()
            .AnyAsync(registro => registro.Nombre == datos.Nombre.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TAmenidad>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Amenidad();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        await _unidadDeTrabajo.Repositorio<Amenidad>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Amenidad con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TAmenidad>> ModificarAsync(
        TAmenidad datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TAmenidad>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Amenidad>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TAmenidad>("No existe el registro a modificar.", 404);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Amenidad>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Nombre == datos.Nombre.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TAmenidad>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        _unidadDeTrabajo.Repositorio<Amenidad>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Amenidad con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    private static string Validar(TAmenidad datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 50)
        {
            return "Nombre es obligatorio y admite hasta 50 caracteres.";
        }

        return string.Empty;
    }

    private static Expression<Func<Amenidad, TAmenidad>> Proyectar()
    {
        return registro => new TAmenidad
        {
            Id = registro.Id,
            Nombre = registro.Nombre
        };
    }
}
