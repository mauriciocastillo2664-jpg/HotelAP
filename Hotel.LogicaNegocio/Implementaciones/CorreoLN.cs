using AutoMapper;
using Hotel.Dominio.Entidades;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Dominio.InterfazLN;
using Hotel.Utilitarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.LogicaNegocio.Implementaciones;

public class CorreoLN : BaseLN<CorreoLN>, ICorreoLN
{
    private readonly IAutorizacionLN _autorizacion;

    private readonly IRestauranteFinanzasAD _procedimientos;

    public CorreoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        IAutorizacionLN autorizacion,
        ILogger<CorreoLN> logger,
        IRestauranteFinanzasAD procedimientos)
        : base(unidadDeTrabajo, logger, mapper)
    {
        _autorizacion = autorizacion;
        _procedimientos = procedimientos;
    }

    // La identidad llega desde claims validados y se contrasta con permisos y pertenencia en la base.
    public async Task<Respuesta<List<TCorreo>>> ListarPorClienteAsync(
        int clienteId,
        int usuarioId,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (clienteId <= 0 || !PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TCorreo>>("Indicá un identificador positivo y una paginación válida.", 400);
        }



        if (!await PuedeConsultarClienteAsync(clienteId, usuarioId, cancellationToken))
        {
            return Fallar<List<TCorreo>>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Correo>().Consultar()
            .Where(registro => registro.ClienteId == clienteId)
            .OrderByDescending(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(registro => new TCorreo
            {
                Id = registro.Id,
                Direccion = registro.Direccion,
                TipoCorreoId = registro.TipoCorreoId,
                EstadoCorreoId = registro.EstadoCorreoId,
                Remitente = registro.Remitente,
                Destinatario = registro.Destinatario,
                Asunto = registro.Asunto,
                Cuerpo = registro.Cuerpo,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                FacturaId = registro.FacturaId,
                CorreoRespuestaId = registro.CorreoRespuestaId,
                Intentos = registro.Intentos,
                MensajeError = registro.MensajeError,
                UsuarioId = registro.UsuarioId,
                FechaRegistro = registro.FechaRegistro,
                FechaEnvio = registro.FechaEnvio
            })
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TCorreo>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TCorreo>> ObtenerAsync(
        int id,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Fallar<TCorreo>("El identificador debe ser positivo.", 400);
        }

        var propietario = await _unidadDeTrabajo.Repositorio<Correo>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new { registro.ClienteId })
            .SingleOrDefaultAsync(cancellationToken);

        if (propietario == null)
        {
            return Fallar<TCorreo>("No existe el registro solicitado.", 404);
        }

        if (!await PuedeConsultarClienteAsync(propietario.ClienteId, usuarioId, cancellationToken))
        {
            return Fallar<TCorreo>("No tenés permiso para consultar estos datos.", 403);
        }

        var datos = await _unidadDeTrabajo.Repositorio<Correo>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(registro => new TCorreo
            {
                Id = registro.Id,
                Direccion = registro.Direccion,
                TipoCorreoId = registro.TipoCorreoId,
                EstadoCorreoId = registro.EstadoCorreoId,
                Remitente = registro.Remitente,
                Destinatario = registro.Destinatario,
                Asunto = registro.Asunto,
                Cuerpo = registro.Cuerpo,
                ClienteId = registro.ClienteId,
                ReservacionId = registro.ReservacionId,
                FacturaId = registro.FacturaId,
                CorreoRespuestaId = registro.CorreoRespuestaId,
                Intentos = registro.Intentos,
                MensajeError = registro.MensajeError,
                UsuarioId = registro.UsuarioId,
                FechaRegistro = registro.FechaRegistro,
                FechaEnvio = registro.FechaEnvio
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TCorreo>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TCorreo>
        {
            Data = datos
        };
    }

    // Registrar el historial no envía el mensaje. El envío SMTP se implementa en el servicio de correo.
    public async Task<Respuesta<TCorreo>> RegistrarAsync(
        TRegistrarCorreo datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0 || datos.TipoCorreoId <= 0 || datos.Direccion is not ("Enviado" or "Recibido"))
        {
            return Fallar<TCorreo>("Indicá el tipo de correo, la dirección y el usuario responsable.", 400);
        }

        if (string.IsNullOrWhiteSpace(datos.Asunto) || datos.Asunto.Length > 200 || string.IsNullOrWhiteSpace(datos.Cuerpo))
        {
            return Fallar<TCorreo>("El asunto es obligatorio, admite hasta 200 caracteres y el cuerpo es obligatorio.", 400);
        }

        if (datos.Remitente?.Length > 150 || datos.Destinatario?.Length > 150)
        {
            return Fallar<TCorreo>("Las direcciones de correo admiten hasta 150 caracteres.", 400);
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<TCorreo>("No tenés permiso para administrar los correos.", 403);
        }

        if ((datos.Remitente != null && !System.Net.Mail.MailAddress.TryCreate(datos.Remitente, out _))
            || (datos.Destinatario != null && !System.Net.Mail.MailAddress.TryCreate(datos.Destinatario, out _)))
        {
            return Fallar<TCorreo>("Indicá direcciones de correo válidas.", 400);
        }

        var resultado = await _procedimientos.RegistrarCorreoAsync(datos, usuarioId, cancellationToken);
        if (!resultado.Success)
        {
            return Fallar<TCorreo>(resultado.Error, resultado.EstadoHttp);
        }

        return await ObtenerAsync(resultado.Data, usuarioId, cancellationToken);
    }

    public async Task<Respuesta<bool>> ActualizarEstadoAsync(
        TActualizarEstadoCorreo datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        if (datos.CorreoId <= 0 || datos.MensajeError?.Length > 500)
        {
            return Fallar<bool>("Indicá el correo y un mensaje de error de hasta 500 caracteres.", 400);
        }

        if (!await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
        {
            return Fallar<bool>("No tenés permiso para administrar los correos.", 403);
        }

        return await _procedimientos.ActualizarEstadoCorreoAsync(datos, cancellationToken);
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

        if (await _autorizacion.TienePermisoAsync(usuarioId, "Usuarios.Gestionar", cancellationToken))
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
