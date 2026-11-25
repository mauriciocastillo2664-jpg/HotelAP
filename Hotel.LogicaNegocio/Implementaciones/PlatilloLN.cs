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

public class PlatilloLN : BaseLN<PlatilloLN>, IPlatilloLN
{
    public PlatilloLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<PlatilloLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TPlatillo>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TPlatillo>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TPlatillo>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Platillo>().Consultar();

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

        return new Respuesta<List<TPlatillo>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TPlatillo>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Platillo>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TPlatillo>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TPlatillo>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TPlatillo>> InsertarAsync(
        TPlatillo datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TPlatillo>(error, 400);
        }

        if (!await _unidadDeTrabajo.Repositorio<CategoriaPlatillo>().Consultar()
            .AnyAsync(registro => registro.Id == datos.CategoriaId && registro.Activo, cancellationToken))
        {
            return Fallar<TPlatillo>("La referencia CategoriaId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Platillo>().Consultar()
            .AnyAsync(registro => registro.Nombre == datos.Nombre.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TPlatillo>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Platillo();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = true;
        await _unidadDeTrabajo.Repositorio<Platillo>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Platillo con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TPlatillo>> ModificarAsync(
        TPlatillo datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TPlatillo>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Platillo>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TPlatillo>("No existe el registro a modificar.", 404);
        }

        if (!await _unidadDeTrabajo.Repositorio<CategoriaPlatillo>().Consultar()
            .AnyAsync(registro => registro.Id == datos.CategoriaId && registro.Activo, cancellationToken))
        {
            return Fallar<TPlatillo>("La referencia CategoriaId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Platillo>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Nombre == datos.Nombre.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TPlatillo>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<Platillo>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Platillo con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<Platillo>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<Platillo>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó Platillo con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TPlatillo datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 100)
        {
            return "Nombre es obligatorio y admite hasta 100 caracteres.";
        }

        if (datos.Descripcion?.Length > 300)
        {
            return "Descripcion admite hasta 300 caracteres.";
        }

        if (datos.ImagenUrl?.Length > 300)
        {
            return "ImagenUrl admite hasta 300 caracteres.";
        }

        if (datos.Precio <= 0 || datos.Precio > 99999999.99m || decimal.Round(datos.Precio, 2) != datos.Precio)
        {
            return "El precio debe ser positivo, con hasta dos decimales y dentro del límite permitido.";
        }

        if (datos.TiempoPreparacion < 0)
        {
            return "El tiempo de preparación debe ser no negativo.";
        }

        if (!string.IsNullOrWhiteSpace(datos.ImagenUrl) && (!Uri.TryCreate(datos.ImagenUrl, UriKind.Absolute, out var imagen) || (imagen.Scheme != Uri.UriSchemeHttps && imagen.Scheme != Uri.UriSchemeHttp)))
        {
            return "La imagen debe tener una dirección HTTP o HTTPS válida.";
        }

        return string.Empty;
    }

    private static Expression<Func<Platillo, TPlatillo>> Proyectar()
    {
        return registro => new TPlatillo
        {
            Id = registro.Id,
            CategoriaId = registro.CategoriaId,
            Nombre = registro.Nombre,
            Descripcion = registro.Descripcion,
            Precio = registro.Precio,
            ImagenUrl = registro.ImagenUrl,
            TiempoPreparacion = registro.TiempoPreparacion,
            EsBebida = registro.EsBebida,
            Disponible = registro.Disponible,
            Activo = registro.Activo
        };
    }
}
