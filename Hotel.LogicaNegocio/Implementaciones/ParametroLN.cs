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

public class ParametroLN : BaseLN<ParametroLN>, IParametroLN
{
    public ParametroLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IMapper mapper,
        ILogger<ParametroLN> logger)
        : base(unidadDeTrabajo, logger, mapper)
    {
    }

    public Task<Respuesta<List<TParametro>>> ListarAsync(
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        return BuscarAsync(string.Empty, pagina, tamanoPagina, cancellationToken);
    }

    public async Task<Respuesta<List<TParametro>>> BuscarAsync(
        string texto,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        if (!PaginacionValida(pagina, tamanoPagina))
        {
            return Fallar<List<TParametro>>("La página debe ser positiva y el tamaño debe estar entre 1 y 100.", 400);
        }

        string filtro = texto?.Trim() ?? string.Empty;
        var consulta = _unidadDeTrabajo.Repositorio<Parametro>().Consultar();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            consulta = consulta.Where(registro => registro.Clave.Contains(filtro));
        }

        var datos = await consulta
            .OrderBy(registro => registro.Clave)
            .ThenBy(registro => registro.Id)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Proyectar())
            .ToListAsync(cancellationToken);

        return new Respuesta<List<TParametro>>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TParametro>> ObtenerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var datos = await _unidadDeTrabajo.Repositorio<Parametro>().Consultar()
            .Where(registro => registro.Id == id)
            .Select(Proyectar())
            .SingleOrDefaultAsync(cancellationToken);

        if (datos == null)
        {
            return Fallar<TParametro>("No existe el registro solicitado.", 404);
        }

        return new Respuesta<TParametro>
        {
            Data = datos
        };
    }

    public async Task<Respuesta<TParametro>> InsertarAsync(
        TParametro datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TParametro>(error, 400);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Parametro>().Consultar()
            .AnyAsync(registro => registro.Clave == datos.Clave.Trim(), cancellationToken);

        if (existe)
        {
            return Fallar<TParametro>("Ya existe un registro con esos datos únicos.", 409);
        }

        var entidad = new Parametro();

        _mapper.Map(datos, entidad);
        entidad.Clave = datos.Clave.Trim();
        entidad.Valor = datos.Valor.Trim();
        entidad.TipoDato = datos.TipoDato.Trim();
        entidad.FechaModificacion = DateTime.UtcNow;
        await _unidadDeTrabajo.Repositorio<Parametro>().InsertarAsync(entidad, cancellationToken);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Parametro con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    public async Task<Respuesta<TParametro>> ModificarAsync(
        TParametro datos,
        CancellationToken cancellationToken = default)
    {
        string error = Validar(datos);
        if (!string.IsNullOrEmpty(error))
        {
            return Fallar<TParametro>(error, 400);
        }

        var entidad = await _unidadDeTrabajo.Repositorio<Parametro>().ObtenerEntidadAsync(
            registro => registro.Id == datos.Id, cancellationToken);

        if (entidad == null)
        {
            return Fallar<TParametro>("No existe el registro a modificar.", 404);
        }

        bool existe = await _unidadDeTrabajo.Repositorio<Parametro>().Consultar()
            .AnyAsync(registro => registro.Id != datos.Id && (registro.Clave == datos.Clave.Trim()), cancellationToken);

        if (existe)
        {
            return Fallar<TParametro>("Ya existe un registro con esos datos únicos.", 409);
        }

        if (!string.Equals(entidad.Clave, datos.Clave.Trim(), StringComparison.Ordinal))
        {
            return Fallar<TParametro>("La clave de un parámetro existente se conserva para mantener las reglas del sistema.", 400);
        }

        _mapper.Map(datos, entidad);
        entidad.Clave = datos.Clave.Trim();
        entidad.Valor = datos.Valor.Trim();
        entidad.TipoDato = datos.TipoDato.Trim();
        entidad.FechaModificacion = DateTime.UtcNow;
        _unidadDeTrabajo.Repositorio<Parametro>().Modificar(entidad);
        await _unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);
        _logger.LogInformation("Se guardó Parametro con identificador {Id}.", entidad.Id);

        return await ObtenerAsync(entidad.Id, cancellationToken);
    }

    private static string Validar(TParametro datos)
    {
        if (datos == null)
        {
            return "Los datos del registro son obligatorios.";
        }

        if (string.IsNullOrWhiteSpace(datos.Clave) || datos.Clave.Trim().Length > 50)
        {
            return "Clave es obligatorio y admite hasta 50 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.Valor) || datos.Valor.Trim().Length > 100)
        {
            return "Valor es obligatorio y admite hasta 100 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(datos.TipoDato) || datos.TipoDato.Trim().Length > 10)
        {
            return "TipoDato es obligatorio y admite hasta 10 caracteres.";
        }

        if (datos.Descripcion?.Length > 200)
        {
            return "Descripcion admite hasta 200 caracteres.";
        }

        if (datos.TipoDato != "Decimal" && datos.TipoDato != "Entero" && datos.TipoDato != "Hora" && datos.TipoDato != "Texto")
        {
            return "El tipo debe ser Decimal, Entero, Hora o Texto.";
        }

        if (datos.TipoDato == "Decimal" && !decimal.TryParse(datos.Valor, System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            return "El valor decimal debe usar el formato invariable con punto decimal.";
        }

        if (datos.TipoDato == "Entero" && !int.TryParse(datos.Valor, out _))
        {
            return "El valor debe ser un entero válido.";
        }

        if (datos.TipoDato == "Hora" && !TimeOnly.TryParseExact(datos.Valor, "HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
        {
            return "La hora debe tener formato HH:mm.";
        }

        string clave = datos.Clave.Trim();
        bool esPorcentaje = clave == "IVA" || clave == "PorcentajeAnticipo"
            || clave == "PorcentajeLateCheckout" || clave == "RecargoFinDeSemana"
            || clave == "RecargoRoomService";

        if (esPorcentaje)
        {
            if (datos.TipoDato != "Decimal" || !decimal.TryParse(datos.Valor,
                System.Globalization.NumberStyles.AllowDecimalPoint,
                System.Globalization.CultureInfo.InvariantCulture, out decimal porcentaje)
                || porcentaje < 0 || porcentaje > 100)
            {
                return "Los parámetros porcentuales deben ser decimales entre cero y cien.";
            }
        }

        bool esCantidad = clave == "MaxNochesPorReserva" || clave == "MaxIntentosCorreo"
            || clave == "MinutosTokenRecuperacion" || clave == "HorasTokenVerificacion";

        if (esCantidad && (datos.TipoDato != "Entero"
            || !int.TryParse(datos.Valor, out int cantidad) || cantidad < 1))
        {
            return "El parámetro de cantidad debe ser un entero positivo.";
        }

        if (clave == "HorasCancelacionSinCargo" && (datos.TipoDato != "Entero"
            || !int.TryParse(datos.Valor, out int horas) || horas < 0))
        {
            return "Las horas de cancelación deben ser un entero no negativo.";
        }

        if ((clave == "HoraCheckIn" || clave == "HoraCheckOut") && datos.TipoDato != "Hora")
        {
            return "Las horas operativas deben usar el tipo Hora.";
        }

        return string.Empty;
    }

    private static Expression<Func<Parametro, TParametro>> Proyectar()
    {
        return registro => new TParametro
        {
            Id = registro.Id,
            Clave = registro.Clave,
            Valor = registro.Valor,
            TipoDato = registro.TipoDato,
            Descripcion = registro.Descripcion,
            FechaModificacion = registro.FechaModificacion
        };
    }
}
