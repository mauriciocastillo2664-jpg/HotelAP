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

public class EstadoCorreoLN : BaseLN<EstadoCorreoLN>, IEstadoCorreoLN
{
    public EstadoCorreoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<EstadoCorreoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TEstadoCorreo>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TEstadoCorreo>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TEstadoCorreo>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<EstadoCorreo>().Consultar();

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

        return new Respuesta<List<TEstadoCorreo>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TEstadoCorreo>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<EstadoCorreo>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TEstadoCorreo>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TEstadoCorreo>
        {
            Data = datos
        };
    }

    private static Expression<Func<EstadoCorreo, TEstadoCorreo>> Proyectar()
    {
        return registro => new TEstadoCorreo
        {
            Id = registro.Id,
            Nombre = registro.Nombre
        };
    }
}
