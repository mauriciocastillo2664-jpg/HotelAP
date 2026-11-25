using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class PedidoRestauranteLN : BaseLN<PedidoRestauranteLN>, IPedidoRestauranteLN
{
    private readonly IAutorizacionLN _autorizacion;

    private readonly IRestauranteFinanzasAD _procedimientos;

    public PedidoRestauranteLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<PedidoRestauranteLN> logger,
        IRestauranteFinanzasAD procedimientos)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
        _procedimientos = procedimientos;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TPedidoRestaurante>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (clienteId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TPedidoRestaurante>>("Indicá un identificador positivo y una paginación válida.", 400);
        }



        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TPedidoRestaurante>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<PedidoRestaurante>().Consultar()
            .Where(registro => registro.ClienteId == clienteId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TPedidoRestaurante
            {
                Id = registro.Id,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                HabitacionId = registro.HabitacionId,
                Modalidad = registro.Modalidad,
                CargoAhabitacion = registro.CargoAhabitacion,
                EstadoPedidoId = registro.EstadoPedidoId,
                DescuentoId = registro.DescuentoId,
                FechaPedido = registro.FechaPedido,
                FechaHoraDeseada = registro.FechaHoraDeseada,
                Instrucciones = registro.Instrucciones,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Recargo = registro.Recargo,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                UsuarioId = registro.UsuarioId,
                RowVersion = registro.RowVersion
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TPedidoRestaurante>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TPedidoRestaurante>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TPedidoRestaurante>("El identificador debe ser positivo.", 400);
        }

        int? clienteId = await _unidadDeTrabajo.Repositorio<PedidoRestaurante>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => (int?)registro.ClienteId)
            .SingleOrDefaultAsync(cancellationToken);

        if (clienteId == null)
        {
            return Fallar<TPedidoRestaurante>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<TPedidoRestaurante>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<PedidoRestaurante>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TPedidoRestaurante
            {
                Id = registro.Id,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                HabitacionId = registro.HabitacionId,
                Modalidad = registro.Modalidad,
                CargoAhabitacion = registro.CargoAhabitacion,
                EstadoPedidoId = registro.EstadoPedidoId,
                DescuentoId = registro.DescuentoId,
                FechaPedido = registro.FechaPedido,
                FechaHoraDeseada = registro.FechaHoraDeseada,
                Instrucciones = registro.Instrucciones,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Recargo = registro.Recargo,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                UsuarioId = registro.UsuarioId,
                RowVersion = registro.RowVersion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TPedidoRestaurante>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TPedidoRestaurante>
        {
            Data = datos
        };
    }

    // El usuarioId debe provenir de la identidad validada por la API, con permiso para esta operación.
    public async Task<Respuesta<TPedidoRestaurante>> CrearAsync(
        TCrearPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.ClienteId <= 0)
        {
            return Fallar<TPedidoRestaurante>("Indicá el cliente y el usuario responsable.", 400);
        }

        if (datos.Modalidad is not ("RoomService" or "Mesa" or "Piscina"))
        {
            return Fallar<TPedidoRestaurante>("La modalidad debe ser RoomService, Mesa o Piscina.", 400);
        }

        if (datos.Instrucciones?.Length > 300 || datos.CodigoDescuento?.Length > 20)
        {
            return Fallar<TPedidoRestaurante>("Las instrucciones o el código de descuento exceden el tamaño permitido.", 400);
        }

        if (!await PuedeCrearParaClienteAsync(datos.ClienteId, usuarioId, cancellationToken))
        {
            return Fallar<TPedidoRestaurante>("No tenés permiso para crear pedidos para este cliente.", 403);
        }

        if (datos.FechaHoraDeseada.HasValue && datos.FechaHoraDeseada.Value < DateTime.Now)
        {
            return Fallar<TPedidoRestaurante>("La fecha deseada no puede estar en el pasado.", 400);
        }

        var resultado = await _procedimientos.CrearPedidoAsync(datos, usuarioId, cancellationToken);
        if (!resultado.Success)
        {
            return Fallar<TPedidoRestaurante>(resultado.Error, resultado.EstadoHttp);
        }

        return await ObtenerAsync(resultado.Data, usuarioId, cancellationToken);
    }

    public async Task<Respuesta<bool>> AgregarPlatilloAsync(
        TAgregarPlatilloPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.PedidoId <= 0 || datos.PlatilloId <= 0 || datos.Cantidad <= 0)
        {
            return Fallar<bool>("Indicá un pedido, un platillo, una cantidad positiva y el usuario responsable.", 400);
        }

        if (!await PuedeModificarPedidoAsync(datos.PedidoId, usuarioId, cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para modificar este pedido.", 403);
        }

        return await _procedimientos.AgregarPlatilloPedidoAsync(datos, cancellationToken);
    }

    public async Task<Respuesta<bool>> CambiarEstadoAsync(
        TCambiarEstadoPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.PedidoId <= 0 || datos.NuevoEstadoId <= 0)
        {
            return Fallar<bool>("Indicá el pedido, el estado y el usuario responsable.", 400);
        }

        bool gestiona = await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Gestionar", cancellationToken);
        if (!gestiona)
        {
            bool esCancelacion = await _unidadDeTrabajo.Repositorio<EstadoPedido>().Consultar()
                .AnyAsync(estado => estado.Id == datos.NuevoEstadoId && estado.Nombre == "Cancelado", cancellationToken);

            if (!esCancelacion || !await EsPedidoPropioAsync(datos.PedidoId, usuarioId, cancellationToken))
            {
                return Fallar<bool>("Solo podés cancelar tus propios pedidos cuando el estado lo permita.", 403);
            }
        }

        return await _procedimientos.CambiarEstadoPedidoAsync(datos, usuarioId, cancellationToken);
    }

    public async Task<Respuesta<TFactura>> PagarAsync(
        TPagarPedido datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.PedidoId <= 0 || datos.MetodoPagoId <= 0)
        {
            return Fallar<TFactura>("Indicá el pedido, el método de pago y el usuario responsable.", 400);
        }

        if (!PagoValido(datos.Referencia, datos.UltimosDigitos))
        {
            return Fallar<TFactura>("La referencia admite 100 caracteres y los últimos dígitos deben ser cuatro números.", 400);
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Pagos.Registrar", cancellationToken))
        {
            return Fallar<TFactura>("No tenés permiso para registrar pagos.", 403);
        }

        var resultado = await _procedimientos.PagarPedidoAsync(datos, usuarioId, cancellationToken);
        if (!resultado.Success)
        {
            return Fallar<TFactura>(resultado.Error, resultado.EstadoHttp);
        }

        var factura = await _unidadDeTrabajo.Repositorio<Factura>().Consultar()
            .Where(registro => registro.Id == resultado.Data)
            .Select(registro => new TFactura
            {
                Id = registro.Id,
                NumeroFactura = registro.NumeroFactura,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                PedidoId = registro.PedidoId,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                TotalPagado = registro.TotalPagado,
                FechaEmision = registro.FechaEmision,
                UsuarioId = registro.UsuarioId
            })
            .SingleAsync(cancellationToken);

        return new Respuesta<TFactura>
        {
            Data = factura
        };
    }

    private static bool PagoValido(string? referencia, string? ultimosDigitos)
    {
        if (referencia?.Length > 100)
        {
            return false;
        }

        if (ultimosDigitos != null && (ultimosDigitos.Length != 4 || !ultimosDigitos.All(char.IsAsciiDigit)))
        {
            return false;
        }

        return true;
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

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Gestionar", cancellationToken))
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

    private async Task<bool> PuedeCrearParaClienteAsync(
        int clienteId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Crear", cancellationToken))
        {
            return true;
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Propios", cancellationToken))
        {
            return false;
        }

        return await _unidadDeTrabajo.Repositorio<Usuario>().Consultar()
            .AnyAsync(usuario => usuario.Id == usuarioId && usuario.ClienteId == clienteId, cancellationToken);
    }

    private async Task<bool> EsPedidoPropioAsync(
        int pedidoId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Propios", cancellationToken))
        {
            return false;
        }

        return await _unidadDeTrabajo.Repositorio<PedidoRestaurante>().Consultar()
            .AnyAsync(pedido => pedido.Id == pedidoId && pedido.Cliente.Usuario != null && pedido.Cliente.Usuario.Id == usuarioId, cancellationToken);
    }

    private async Task<bool> PuedeModificarPedidoAsync(
        int pedidoId,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Gestionar", cancellationToken))
        {
            return true;
        }

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Pedidos.Crear", cancellationToken))
        {
            return true;
        }

        return await EsPedidoPropioAsync(pedidoId, usuarioId, cancellationToken);
    }
}
