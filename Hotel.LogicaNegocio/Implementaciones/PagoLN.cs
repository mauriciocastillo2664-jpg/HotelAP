using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class PagoLN : BaseLN<PagoLN>, IPagoLN
{
    private readonly IAutorizacionLN _autorizacion;

    public PagoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<PagoLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TPago>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (clienteId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TPago>>("Indicá un identificador positivo y una paginación válida.", 400);
        }



        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TPago>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Pago>().Consultar()
            .Where(registro => registro.ClienteId == clienteId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TPago
            {
                Id = registro.Id,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                PedidoId = registro.PedidoId,
                MontoTotal = registro.MontoTotal,
                TotalAbonado = registro.TotalAbonado,
                Saldo = registro.Saldo,
                EstadoPagoId = registro.EstadoPagoId,
                FacturaId = registro.FacturaId,
                FechaCreacion = registro.FechaCreacion,
                FechaActualizacion = registro.FechaActualizacion,
                RowVersion = registro.RowVersion
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TPago>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TPago>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TPago>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<Pago>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TPago>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TPago>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Pago>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TPago
            {
                Id = registro.Id,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                PedidoId = registro.PedidoId,
                MontoTotal = registro.MontoTotal,
                TotalAbonado = registro.TotalAbonado,
                Saldo = registro.Saldo,
                EstadoPagoId = registro.EstadoPagoId,
                FacturaId = registro.FacturaId,
                FechaCreacion = registro.FechaCreacion,
                FechaActualizacion = registro.FechaActualizacion,
                RowVersion = registro.RowVersion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TPago>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TPago>
        {
            Data = datos
        };
    }


    private async Task<bool> PuedeConsultarClienteAsync(
        int? clienteId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        if (!await _autorizacion.UsuarioActivoAsync(usuarioId, cancellationToken))
        {
            return false;
        }

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pagos.Leer", cancellationToken))
        {
            return true;
        }

        if (clienteId == null)
        {
            return false;
        }

        return await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId && usuario.ClienteId == clienteId, cancellationToken);
    }
}
