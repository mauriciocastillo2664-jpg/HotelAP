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

public class TemporadaLN : BaseLN<TemporadaLN>, ITemporadaLN
{
    public TemporadaLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<TemporadaLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TTemporada>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TTemporada>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TTemporada>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Temporada>().Consultar();

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

        return new Respuesta<List<TTemporada>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTemporada>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Temporada>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TTemporada>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TTemporada>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TTemporada>> InsertarAsync(
        TTemporada datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTemporada>(error, 400);
        }

        var entidad = new Temporada();

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = true;
        await _unidadDeTrabajo.Repositorio<Temporada>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Temporada con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TTemporada>> ModificarAsync(
        TTemporada datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TTemporada>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Temporada>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TTemporada>("No existe el registro a modificar.", 404);
        }

        _mapper.Map(datos, entidad);
        entidad.Nombre = datos.Nombre.Trim();
        entidad.Activo = datos.Activo;
        _unidadDeTrabajo.Repositorio<Temporada>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Temporada con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<bool>> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entidad = await _unidadDeTrabajo.Repositorio<Temporada>().ObtenerEntidadAsync(
            registro => registro.Id == id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<bool>("No existe el registro a desactivar.", 404);
        }

        entidad.Activo = false;
        _unidadDeTrabajo.Repositorio<Temporada>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se desactivó Temporada con identificador {Id}.", entidad.Id);

        return new Respuesta<bool>
        {
            Data = true
        };
    }

    private static string Validar(TTemporada datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombre) || datos.Nombre.Trim().Length > 60)
        {
            return "Nombre es obligatorio y admite hasta 60 caracteres.";
        }

        if (datos.FechaInicio > datos.FechaFin)
        {
            return "La fecha inicial debe ser anterior o igual a la final.";
        }

        if (datos.PorcentajeRecargo < 0 || datos.PorcentajeRecargo > 999.99m || decimal.Round(datos.PorcentajeRecargo, 2) != datos.PorcentajeRecargo)
        {
            return "El recargo debe estar entre cero y 999.99 con hasta dos decimales.";
        }

        return string.Empty;
    }

    private static Expression<Func<Temporada, TTemporada>> Proyectar()
    {
        return registro => new TTemporada
        {
            Id = registro.Id,
            Nombre = registro.Nombre,
            FechaInicio = registro.FechaInicio,
            FechaFin = registro.FechaFin,
            PorcentajeRecargo = registro.PorcentajeRecargo,
            Activo = registro.Activo
        };
    }
}
