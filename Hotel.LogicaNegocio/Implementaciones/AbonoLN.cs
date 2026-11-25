using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class AbonoLN : BaseLN<AbonoLN>, IAbonoLN
{
    private readonly IAutorizacionLN _autorizacion;

    private readonly IRestauranteFinanzasAD _procedimientos;

    public AbonoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<AbonoLN> logger,
        IRestauranteFinanzasAD procedimientos)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
        _procedimientos = procedimientos;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TAbono>>> ListarPorPagoAsync(
        int pagoId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (pagoId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TAbono>>("Indicá un identificador positivo y una paginación válida.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<Pago>().Consultar()
            .Where(registro => registro.Id == pagoId)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TAbono>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Abono>().Consultar()
            .Where(registro => registro.PagoId == pagoId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TAbono
            {
                Id = registro.Id,
                PagoId = registro.PagoId,
                MetodoPagoId = registro.MetodoPagoId,
                Monto = registro.Monto,
                Referencia = registro.Referencia,
                UltimosDigitosTarjeta = registro.UltimosDigitosTarjeta,
                Estado = registro.Estado,
                MotivoReverso = registro.MotivoReverso,
                FechaAbono = registro.FechaAbono,
                UsuarioId = registro.UsuarioId
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TAbono>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TAbono>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TAbono>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<Abono>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.Pago.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TAbono>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TAbono>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Abono>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TAbono
            {
                Id = registro.Id,
                PagoId = registro.PagoId,
                MetodoPagoId = registro.MetodoPagoId,
                Monto = registro.Monto,
                Referencia = registro.Referencia,
                UltimosDigitosTarjeta = registro.UltimosDigitosTarjeta,
                Estado = registro.Estado,
                MotivoReverso = registro.MotivoReverso,
                FechaAbono = registro.FechaAbono,
                UsuarioId = registro.UsuarioId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TAbono>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TAbono>
        {
            Data = datos
        };
    }

    // La API debe resolver usuarioId desde los claims y autorizar Pagos.Registrar o Pagos.Reversar.
    public async Task<Respuesta<bool>> RegistrarAsync(
        TRegistrarAbono datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.ReservacionId <= 0 || datos.MetodoPagoId <= 0)
        {
            return Fallar<bool>("Indicá la reservación, el método de pago y el usuario responsable.", 400);
        }

        if (datos.Monto <= 0 || datos.Monto > 99999999.99m || decimal.Round(datos.Monto, 2) != datos.Monto)
        {
            return Fallar<bool>("El monto debe ser positivo y tener hasta dos decimales.", 400);
        }

        if (datos.Referencia?.Length > 100)
        {
            return Fallar<bool>("La referencia admite hasta 100 caracteres.", 400);
        }

        if (datos.UltimosDigitos != null && (datos.UltimosDigitos.Length != 4 || !datos.UltimosDigitos.All(char.IsAsciiDigit)))
        {
            return Fallar<bool>("Solo se permiten los últimos cuatro dígitos de la tarjeta.", 400);
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Pagos.Registrar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para realizar esta operación sobre abonos.", 403);
        }

        return await _procedimientos.RegistrarAbonoAsync(datos, usuarioId, cancellationToken);
    }

    public async Task<Respuesta<bool>> ReversarAsync(
        TReversarAbono datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.AbonoId <= 0 || string.IsNullOrWhiteSpace(datos.Motivo) || datos.Motivo.Length > 300)
        {
            return Fallar<bool>("Indicá el abono, el usuario y un motivo de hasta 300 caracteres.", 400);
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Pagos.Reversar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para realizar esta operación sobre abonos.", 403);
        }

        return await _procedimientos.ReversarAbonoAsync(datos, usuarioId, cancellationToken);
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
