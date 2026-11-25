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

public class CategoriaPlatilloLN : BaseLN<CategoriaPlatilloLN>, ICategoriaPlatilloLN
{
    public CategoriaPlatilloLN(IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<CategoriaPlatilloLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TCategoriaPlatillo>>> ListarAsync(
        int pagina = 1, int tamanoPagina = 20, CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TCategoriaPlatillo>>> BuscarAsync(
        string texto, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TCategoriaPlatillo>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string nombre = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.TCategoriaPlatillo.Consultar();

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

        return new Respuesta<List<TCategoriaPlatillo>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TCategoriaPlatillo>> ObtenerAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.TCategoriaPlatillo.Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TCategoriaPlatillo>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TCategoriaPlatillo>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TCategoriaPlatillo>> InsertarAsync(
        TCategoriaPlatillo datos, CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TCategoriaPlatillo>(error, 400);
        }

        string nombre = datos.Nombre.Trim();
        bool existe = await _unidadDeTrabajo.TCategoriaPlatillo.Consultar()
            .AnyAsync(registro => registro.Nombre == nombre, cancellationToken);

        if (existe)
        {
            return Fallar<TCategoriaPlatillo>("Ya existe un registro con ese nombre.", 409);
        }

        var entidad = new CategoriaPlatillo();
        _mapper.Map(datos, entidad);
        entidad.Activo = true;

        await _unidadDeTrabajo.TCategoriaPlatillo.InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se creó CategoriaPlatillo con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TCategoriaPlatillo>> ModificarAsync(
        TCategoriaPlatillo datos, CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TCategoriaPlatillo>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.TCategoriaPlatillo.ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TCategoriaPlatillo>("No existe el registro a modificar.", 404);
        }

        string nombre = datos.Nombre.Trim();
        bool existe = await _unidadDeTrabajo.TCategoriaPlatillo.Consultar()
            .AnyAsync(registro => registro.Nombre == nombre && registro.Id != datos.Id, cancellationToken);

        if (existe)
        {
            return Fallar<TCategoriaPlatillo>("Ya existe otro registro con ese nombre.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.TCategoriaPlatillo.Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se modificó CategoriaPlatillo con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.TCategoriaPlatillo.ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.TCategoriaPlatillo.Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó CategoriaPlatillo con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TCategoriaPlatillo datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 50)
        {
            return "El nombre es obligatorio y admite hasta 50 caracteres.";
        }

        return string.Empty;
    }

    private static Expression<Func<CategoriaPlatillo, TCategoriaPlatillo>> Proyectar()
    {
        return registro => new TCategoriaPlatillo
        {
            Id = registro.Id,
            Nombre = registro.Nombre,

            Activo = registro.Activo
        };
    }
}
