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

public class ClienteLN : BaseLN<ClienteLN>, IClienteLN
{
    public ClienteLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<ClienteLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TCliente>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TCliente>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TCliente>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Cliente>().Consultar();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            consulta = consulta.Where(registro => registro.Nombres.Contains(filtro)
                || registro.Apellidos.Contains(filtro)
                || registro.Email.Contains(filtro)
                || registro.NumeroIdentificacion.Contains(filtro));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Nombres)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TCliente>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TCliente>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Cliente>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TCliente>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TCliente>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TCliente>> InsertarAsync(
        TCliente datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TCliente>(error, 400);
        }

        if (!await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.TipoIdentificacionId && registro.Activo, cancellationToken))
        {
            return Fallar<TCliente>("La referencia TipoIdentificacionId no existe o está inactiva.", 400);
        }

        if (datos.DescuentoId.HasValue && !await _unidadDeTrabajo.Repositorio<Descuento>().Consultar()
            .AnyAsync(registro => registro.Id == datos.DescuentoId && registro.Activo, cancellationToken))
        {
            return Fallar<TCliente>("La referencia DescuentoId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Cliente>().Consultar()
            .AnyAsync(registro => registro.Email == datos.Email.Trim() || (registro.TipoIdentificacionId == datos.TipoIdentificacionId && registro.NumeroIdentificacion == datos.NumeroIdentificacion.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TCliente>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Cliente();

        _mapper.Map(datos, entidad);
        entidad.NumeroIdentificacion = datos.NumeroIdentificacion.Trim();
        entidad.Nombres = datos.Nombres.Trim();
        entidad.Apellidos = datos.Apellidos.Trim();
        entidad.Email = datos.Email.Trim();
        entidad.EstadoUsuarioId = 1;
        entidad.FechaRegistro = DateTime.UtcNow;
        await _unidadDeTrabajo.Repositorio<Cliente>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Cliente con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TCliente>> ModificarAsync(
        TCliente datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TCliente>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Cliente>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TCliente>("No existe el registro a modificar.", 404);
        }

        if (datos.RowVersion == null || !datos.RowVersion.SequenceEqual(entidad.RowVersion))
        {
            return Fallar<TCliente>("El registro cambió. Actualizá los datos antes de modificarlo.", 409);
        }

        if (!await _unidadDeTrabajo.Repositorio<TipoIdentificacion>().Consultar()
            .AnyAsync(registro => registro.Id == datos.TipoIdentificacionId && registro.Activo, cancellationToken))
        {
            return Fallar<TCliente>("La referencia TipoIdentificacionId no existe o está inactiva.", 400);
        }

        if (datos.DescuentoId.HasValue && !await _unidadDeTrabajo.Repositorio<Descuento>().Consultar()
            .AnyAsync(registro => registro.Id == datos.DescuentoId && registro.Activo, cancellationToken))
        {
            return Fallar<TCliente>("La referencia DescuentoId no existe o está inactiva.", 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Cliente>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Email == datos.Email.Trim() || (registro.TipoIdentificacionId == datos.TipoIdentificacionId && registro.NumeroIdentificacion == datos.NumeroIdentificacion.Trim())), cancellationToken);

        if (existe)
        {
            return Fallar<TCliente>("Ya existe un registro con esos datos únicos.", 409);
        }

        _mapper.Map(datos, entidad);
        entidad.NumeroIdentificacion = datos.NumeroIdentificacion.Trim();
        entidad.Nombres = datos.Nombres.Trim();
        entidad.Apellidos = datos.Apellidos.Trim();
        entidad.Email = datos.Email.Trim();
        _unidadDeTrabajo.Repositorio<Cliente>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Cliente con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    private static string Validar(TCliente datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.NumeroIdentificacion) || datos.NumeroIdentificacion.Trim().Length > 30)
        {
            return "NumeroIdentificacion es obligatorio y admite hasta 30 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.Nombres) || datos.Nombres.Trim().Length > 100)
        {
            return "Nombres es obligatorio y admite hasta 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.Apellidos) || datos.Apellidos.Trim().Length > 100)
        {
            return "Apellidos es obligatorio y admite hasta 100 caracteres.";
        }

        if (datos.Nacionalidad?.Length > 50)
        {
            return "Nacionalidad admite hasta 50 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.Email) || datos.Email.Trim().Length > 150)
        {
            return "Email es obligatorio y admite hasta 150 caracteres.";
        }

        if (datos.Telefono?.Length > 20)
        {
            return "Telefono admite hasta 20 caracteres.";
        }

        if (datos.Preferencias?.Length > 300)
        {
            return "Preferencias admite hasta 300 caracteres.";
        }

        if (!System.Net.Mail.MailAddress.TryCreate(datos.Email, out var correo) || correo.Address != datos.Email.Trim())
        {
            return "Indicá un correo electrónico válido.";
        }

        if (datos.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
        {
            return "La fecha de nacimiento no puede estar en el futuro.";
        }

        return string.Empty;
    }

    private static Expression<Func<Cliente, TCliente>> Proyectar()
    {
        return registro => new TCliente
        {
            Id = registro.Id,
            TipoIdentificacionId = registro.TipoIdentificacionId,
            NumeroIdentificacion = registro.NumeroIdentificacion,
            Nombres = registro.Nombres,
            Apellidos = registro.Apellidos,
            Nacionalidad = registro.Nacionalidad,
            Email = registro.Email,
            Telefono = registro.Telefono,
            FechaNacimiento = registro.FechaNacimiento,
            Preferencias = registro.Preferencias,
            DescuentoId = registro.DescuentoId,
            EstadoUsuarioId = registro.EstadoUsuarioId,
            FechaRegistro = registro.FechaRegistro,
            RowVersion = registro.RowVersion
        };
    }
}
