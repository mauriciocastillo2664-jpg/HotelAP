using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class ReportesLN : BaseLN<ReportesLN>, IReportesLN
{
    private readonly IAutorizacionLN _autorizacion;

    public ReportesLN(
        IUnidadTrabajoEF unidadDeTrabajo, IMapper mapper, ILogger<ReportesLN> logger,
        IAutorizacionLN autorizacion)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
    }

    public async Task<Respuesta<List<TVwEstadoCuenta>>> ConsultarEstadoCuentaAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwEstadoCuenta>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwEstadoCuenta>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwEstadoCuenta>().Consultar()
            .OrderBy(registro => registro.PagoId)
            .ThenBy(registro => registro.Cliente)
            .ThenBy(registro => registro.Concepto)
            .ThenBy(registro => registro.MontoTotal)
            .ThenBy(registro => registro.TotalAbonado)
            .ThenBy(registro => registro.Saldo)
            .ThenBy(registro => registro.EstadoPago)
            .ThenBy(registro => registro.CantidadAbonos)
            .ThenBy(registro => registro.FechaActualizacion)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwEstadoCuenta
            {
                PagoId = registro.PagoId,
                Cliente = registro.Cliente,
                Concepto = registro.Concepto,
                MontoTotal = registro.MontoTotal,
                TotalAbonado = registro.TotalAbonado,
                Saldo = registro.Saldo,
                EstadoPago = registro.EstadoPago,
                CantidadAbonos = registro.CantidadAbonos,
                FechaActualizacion = registro.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwEstadoCuenta>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwEstadoUsuario>>> ConsultarEstadoUsuarioAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TVwEstadoUsuario>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwEstadoUsuario>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwEstadoUsuario>().Consultar()
            .OrderBy(registro => registro.TipoRegistro)
            .ThenBy(registro => registro.Id)
            .ThenBy(registro => registro.Identificador)
            .ThenBy(registro => registro.NombreCompleto)
            .ThenBy(registro => registro.TipoIdentificacion)
            .ThenBy(registro => registro.NumeroIdentificacion)
            .ThenBy(registro => registro.Estado)
            .ThenBy(registro => registro.PermiteAcceso)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwEstadoUsuario
            {
                TipoRegistro = registro.TipoRegistro,
                Id = registro.Id,
                Identificador = registro.Identificador,
                NombreCompleto = registro.NombreCompleto,
                TipoIdentificacion = registro.TipoIdentificacion,
                NumeroIdentificacion = registro.NumeroIdentificacion,
                Estado = registro.Estado,
                PermiteAcceso = registro.PermiteAcceso
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwEstadoUsuario>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwFacturaAbono>>> ConsultarFacturaAbonoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwFacturaAbono>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwFacturaAbono>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwFacturaAbono>().Consultar()
            .OrderBy(registro => registro.FacturaId)
            .ThenBy(registro => registro.NumeroFactura)
            .ThenBy(registro => registro.AbonoId)
            .ThenBy(registro => registro.FechaAbono)
            .ThenBy(registro => registro.MetodoPago)
            .ThenBy(registro => registro.Referencia)
            .ThenBy(registro => registro.UltimosDigitosTarjeta)
            .ThenBy(registro => registro.Monto)
            .ThenBy(registro => registro.Estado)
            .ThenBy(registro => registro.TotalFactura)
            .ThenBy(registro => registro.TotalAbonado)
            .ThenBy(registro => registro.Saldo)
            .ThenBy(registro => registro.EstadoPago)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwFacturaAbono
            {
                FacturaId = registro.FacturaId,
                NumeroFactura = registro.NumeroFactura,
                AbonoId = registro.AbonoId,
                FechaAbono = registro.FechaAbono,
                MetodoPago = registro.MetodoPago,
                Referencia = registro.Referencia,
                UltimosDigitosTarjeta = registro.UltimosDigitosTarjeta,
                Monto = registro.Monto,
                Estado = registro.Estado,
                TotalFactura = registro.TotalFactura,
                TotalAbonado = registro.TotalAbonado,
                Saldo = registro.Saldo,
                EstadoPago = registro.EstadoPago
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwFacturaAbono>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwFacturaConcepto>>> ConsultarFacturaConceptoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwFacturaConcepto>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwFacturaConcepto>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwFacturaConcepto>().Consultar()
            .OrderBy(registro => registro.FacturaId)
            .ThenBy(registro => registro.NumeroFactura)
            .ThenBy(registro => registro.FechaEmision)
            .ThenBy(registro => registro.Cliente)
            .ThenBy(registro => registro.Reservacion)
            .ThenBy(registro => registro.TipoConcepto)
            .ThenBy(registro => registro.Descripcion)
            .ThenBy(registro => registro.Cantidad)
            .ThenBy(registro => registro.PrecioUnitario)
            .ThenBy(registro => registro.Subtotal)
            .ThenBy(registro => registro.SubtotalFactura)
            .ThenBy(registro => registro.Descuento)
            .ThenBy(registro => registro.Impuestos)
            .ThenBy(registro => registro.Total)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwFacturaConcepto
            {
                FacturaId = registro.FacturaId,
                NumeroFactura = registro.NumeroFactura,
                FechaEmision = registro.FechaEmision,
                Cliente = registro.Cliente,
                Reservacion = registro.Reservacion,
                TipoConcepto = registro.TipoConcepto,
                Descripcion = registro.Descripcion,
                Cantidad = registro.Cantidad,
                PrecioUnitario = registro.PrecioUnitario,
                Subtotal = registro.Subtotal,
                SubtotalFactura = registro.SubtotalFactura,
                Descuento = registro.Descuento,
                Impuestos = registro.Impuestos,
                Total = registro.Total
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwFacturaConcepto>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwHistorialCorreo>>> ConsultarHistorialCorreoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwHistorialCorreo>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwHistorialCorreo>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwHistorialCorreo>().Consultar()
            .OrderBy(registro => registro.Id)
            .ThenBy(registro => registro.Direccion)
            .ThenBy(registro => registro.Tipo)
            .ThenBy(registro => registro.Estado)
            .ThenBy(registro => registro.Remitente)
            .ThenBy(registro => registro.Destinatario)
            .ThenBy(registro => registro.Asunto)
            .ThenBy(registro => registro.Cliente)
            .ThenBy(registro => registro.Reservacion)
            .ThenBy(registro => registro.NumeroFactura)
            .ThenBy(registro => registro.CorreoRespuestaId)
            .ThenBy(registro => registro.Intentos)
            .ThenBy(registro => registro.MensajeError)
            .ThenBy(registro => registro.EnviadoPor)
            .ThenBy(registro => registro.FechaRegistro)
            .ThenBy(registro => registro.FechaEnvio)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwHistorialCorreo
            {
                Id = registro.Id,
                Direccion = registro.Direccion,
                Tipo = registro.Tipo,
                Estado = registro.Estado,
                Remitente = registro.Remitente,
                Destinatario = registro.Destinatario,
                Asunto = registro.Asunto,
                Cliente = registro.Cliente,
                Reservacion = registro.Reservacion,
                NumeroFactura = registro.NumeroFactura,
                CorreoRespuestaId = registro.CorreoRespuestaId,
                Intentos = registro.Intentos,
                MensajeError = registro.MensajeError,
                EnviadoPor = registro.EnviadoPor,
                FechaRegistro = registro.FechaRegistro,
                FechaEnvio = registro.FechaEnvio
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwHistorialCorreo>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwIngresosPorMes>>> ConsultarIngresosPorMesAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwIngresosPorMes>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwIngresosPorMes>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwIngresosPorMes>().Consultar()
            .OrderBy(registro => registro.Anio)
            .ThenBy(registro => registro.Mes)
            .ThenBy(registro => registro.MetodoPago)
            .ThenBy(registro => registro.Origen)
            .ThenBy(registro => registro.Total)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwIngresosPorMes
            {
                Anio = registro.Anio,
                Mes = registro.Mes,
                MetodoPago = registro.MetodoPago,
                Origen = registro.Origen,
                Total = registro.Total
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwIngresosPorMes>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwOcupacionActual>>> ConsultarOcupacionActualAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwOcupacionActual>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwOcupacionActual>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwOcupacionActual>().Consultar()
            .OrderBy(registro => registro.Estado)
            .ThenBy(registro => registro.Cantidad)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwOcupacionActual
            {
                Estado = registro.Estado,
                Cantidad = registro.Cantidad
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwOcupacionActual>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwPlatillosMasVendido>>> ConsultarPlatillosMasVendidoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwPlatillosMasVendido>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwPlatillosMasVendido>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwPlatillosMasVendido>().Consultar()
            .OrderBy(registro => registro.Id)
            .ThenBy(registro => registro.Nombre)
            .ThenBy(registro => registro.Categoria)
            .ThenBy(registro => registro.CantidadVendida)
            .ThenBy(registro => registro.TotalVendido)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwPlatillosMasVendido
            {
                Id = registro.Id,
                Nombre = registro.Nombre,
                Categoria = registro.Categoria,
                CantidadVendida = registro.CantidadVendida,
                TotalVendido = registro.TotalVendido
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwPlatillosMasVendido>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwReservacionesDetalle>>> ConsultarReservacionesDetalleAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwReservacionesDetalle>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwReservacionesDetalle>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwReservacionesDetalle>().Consultar()
            .OrderBy(registro => registro.Id)
            .ThenBy(registro => registro.Codigo)
            .ThenBy(registro => registro.Estado)
            .ThenBy(registro => registro.FechaInicio)
            .ThenBy(registro => registro.FechaFin)
            .ThenBy(registro => registro.Noches)
            .ThenBy(registro => registro.Cliente)
            .ThenBy(registro => registro.Habitacion)
            .ThenBy(registro => registro.TipoHabitacion)
            .ThenBy(registro => registro.Subtotal)
            .ThenBy(registro => registro.Descuento)
            .ThenBy(registro => registro.Impuestos)
            .ThenBy(registro => registro.Total)
            .ThenBy(registro => registro.AnticipoRequerido)
            .ThenBy(registro => registro.Saldo)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwReservacionesDetalle
            {
                Id = registro.Id,
                Codigo = registro.Codigo,
                Estado = registro.Estado,
                FechaInicio = registro.FechaInicio,
                FechaFin = registro.FechaFin,
                Noches = registro.Noches,
                Cliente = registro.Cliente,
                Habitacion = registro.Habitacion,
                TipoHabitacion = registro.TipoHabitacion,
                Subtotal = registro.Subtotal,
                Descuento = registro.Descuento,
                Impuestos = registro.Impuestos,
                Total = registro.Total,
                AnticipoRequerido = registro.AnticipoRequerido,
                Saldo = registro.Saldo
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwReservacionesDetalle>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwTiemposPreparacion>>> ConsultarTiemposPreparacionAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Reportes.Ver", cancellationToken))
        {
            return Fallar<List<TVwTiemposPreparacion>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwTiemposPreparacion>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwTiemposPreparacion>().Consultar()
            .OrderBy(registro => registro.PedidoId)
            .ThenBy(registro => registro.InicioPreparacion)
            .ThenBy(registro => registro.Listo)
            .ThenBy(registro => registro.MinutosReales)
            .ThenBy(registro => registro.MinutosEstimados)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwTiemposPreparacion
            {
                PedidoId = registro.PedidoId,
                InicioPreparacion = registro.InicioPreparacion,
                Listo = registro.Listo,
                MinutosReales = registro.MinutosReales,
                MinutosEstimados = registro.MinutosEstimados
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwTiemposPreparacion>>
        {
            Data = datos
        };
    }


    public async Task<Respuesta<List<TVwUsuarioPermiso>>> ConsultarUsuarioPermisoAsync(
        int usuarioId, int pagina = 1, int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<List<TVwUsuarioPermiso>>("No tenés permiso para consultar este reporte.", 403);
        }

        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TVwUsuarioPermiso>>("La página y el tamaño solicitados no son válidos.", 400);
        }

        var datos = await _unidadDeTrabajo.Repositorio<VwUsuarioPermiso>().Consultar()
            .OrderBy(registro => registro.UsuarioId)
            .ThenBy(registro => registro.NombreUsuario)
            .ThenBy(registro => registro.Email)
            .ThenBy(registro => registro.Rol)
            .ThenBy(registro => registro.Permiso)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TVwUsuarioPermiso
            {
                UsuarioId = registro.UsuarioId,
                NombreUsuario = registro.NombreUsuario,
                Email = registro.Email,
                Rol = registro.Rol,
                Permiso = registro.Permiso
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TVwUsuarioPermiso>>
        {
            Data = datos
        };
    }

}
