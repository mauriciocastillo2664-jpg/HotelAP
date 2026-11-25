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

public class DescuentoLN : BaseLN<DescuentoLN>, IDescuentoLN
{
    public DescuentoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<DescuentoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TDescuento>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TDescuento>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TDescuento>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Descuento>().Consultar();

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

        return new Respuesta<List<TDescuento>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TDescuento>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Descuento>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TDescuento>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TDescuento>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TDescuento>> InsertarAsync(
        TDescuento datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TDescuento>(error, 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Descuento>().Consultar()
            .AnyAsync(registro => datos.Codigo != null && registro.Codigo == datos.Codigo.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TDescuento>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Descuento();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Codigo = string.IsNullOrWhiteSpace(datos.Codigo) ? null : datos.Codigo.Trim();
        entidad.Tipo = datos.Tipo.Trim();
        entidad.AplicaA = datos.AplicaA.Trim();
        entidad.Activo = true;
        await _unidadDeTrabajo.Repositorio<Descuento>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Descuento con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TDescuento>> ModificarAsync(
        TDescuento datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TDescuento>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Descuento>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TDescuento>("No existe el registro a modificar.", 404);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Descuento>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (datos.Codigo != null && registro.Codigo == datos.Codigo.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TDescuento>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Codigo = string.IsNullOrWhiteSpace(datos.Codigo) ? null : datos.Codigo.Trim();
        entidad.Tipo = datos.Tipo.Trim();
        entidad.AplicaA = datos.AplicaA.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<Descuento>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Descuento con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<Descuento>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<Descuento>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó Descuento con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TDescuento datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 60)
        {
            return "Nombre es obligatorio y admite hasta 60 caracteres.";
        }

        if (datos.Codigo?.Length > 20)
        {
            return "Codigo admite hasta 20 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.Tipo) || datos.Tipo.Trim().Length > 15)
        {
            return "Tipo es obligatorio y admite hasta 15 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.AplicaA) || datos.AplicaA.Trim().Length > 15)
        {
            return "AplicaA es obligatorio y admite hasta 15 caracteres.";
        }

        if (datos.Tipo != "Porcentaje" && datos.Tipo != "MontoFijo")
        {
            return "El tipo debe ser Porcentaje o MontoFijo.";
        }

        if (datos.AplicaA != "Hospedaje" && datos.AplicaA != "Restaurante" && datos.AplicaA != "Ambos")
        {
            return "Indicá el ámbito válido del descuento.";
        }

        if (datos.Valor <= 0 || datos.Valor > 99999999.99m || decimal.Round(datos.Valor, 2) != datos.Valor || (datos.Tipo == "Porcentaje" && datos.Valor > 100))
        {
            return "El descuento debe ser positivo, con hasta dos decimales y porcentaje máximo de 100.";
        }

        if (datos.FechaInicio.HasValue && datos.FechaFin.HasValue && datos.FechaInicio > datos.FechaFin)
        {
            return "El intervalo de vigencia es inválido.";
        }

        return string.Empty;
    }

    private static Expression<Func<Descuento, TDescuento>> Proyectar()
    {
        return registro => new TDescuento
        {
            Id = registro.Id,
            Nombre = registro.Nombre,
            Codigo = registro.Codigo,
            Tipo = registro.Tipo,
            Valor = registro.Valor,
            AplicaA = registro.AplicaA,
            FechaInicio = registro.FechaInicio,
            FechaFin = registro.FechaFin,
            Activo = registro.Activo
        };
    }
}
