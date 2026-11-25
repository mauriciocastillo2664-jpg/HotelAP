using System.Data;
using System.Data.Common;
using Hotel.AccesoDatos.Contexto;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Utilitarios;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hotel.AccesoDatos.Implementaciones;

public class HospedajeAD : IHospedajeAD
{
    private readonly HotelContext _contexto;

    public HospedajeAD(HotelContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<Respuesta<int>> CrearReservacionAsync(
        TCrearReservacion datos,
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        var parametros = new Dictionary<string, object?>
        {
            ["@ClienteId"] = datos.ClienteId,
            ["@HabitacionId"] = datos.HabitacionId,
            ["@Inicio"] = datos.FechaInicio.ToDateTime(TimeOnly.MinValue),
            ["@Fin"] = datos.FechaFin.ToDateTime(TimeOnly.MinValue),
            ["@Adultos"] = datos.CantidadAdultos,
            ["@Ninos"] = datos.CantidadNinos,
            ["@CodigoDescuento"] = datos.CodigoDescuento,
            ["@Observaciones"] = datos.Observaciones,
            ["@UsuarioId"] = usuarioId
        };

        try
        {
            await using var comando = PrepararComando("sp_CrearReservacion", parametros);
            bool cerrarConexion = await AbrirConexionAsync(cancellationToken);
            try
            {
                object? valor = await comando.ExecuteScalarAsync(cancellationToken);
                return new Respuesta<int>
                {
                    Data = Convert.ToInt32(valor)
                };
            }
            finally
            {
                if (cerrarConexion)
                {
                    await _contexto.Database.CloseConnectionAsync();
                }
            }
        }
        catch (SqlException exception) when (EsErrorNegocio(exception))
        {
            return ErrorNegocio<int>(exception);
        }
    }

    public async Task<Respuesta<bool>> EjecutarOperacionAsync(
        OperacionHospedaje operacion,
        IReadOnlyDictionary<string, object?> parametros,
        CancellationToken cancellationToken = default)
    {
        string procedimiento = operacion switch
        {
            OperacionHospedaje.ModificarReservacion => "sp_ModificarReservacion",
            OperacionHospedaje.CancelarReservacion => "sp_CancelarReservacion",
            OperacionHospedaje.CheckIn => "sp_CheckIn",
            OperacionHospedaje.CheckOut => "sp_CheckOut",
            OperacionHospedaje.RegistrarCargoAdicional => "sp_RegistrarCargoAdicional",
            OperacionHospedaje.RegistrarAjuste => "sp_RegistrarAjuste",
            _ => throw new ArgumentOutOfRangeException(nameof(operacion))
        };

        try
        {
            await using var comando = PrepararComando(procedimiento, parametros);
            bool cerrarConexion = await AbrirConexionAsync(cancellationToken);
            try
            {
                await comando.ExecuteNonQueryAsync(cancellationToken);
                return new Respuesta<bool>
                {
                    Data = true
                };
            }
            finally
            {
                if (cerrarConexion)
                {
                    await _contexto.Database.CloseConnectionAsync();
                }
            }
        }
        catch (SqlException exception) when (EsErrorNegocio(exception))
        {
            return ErrorNegocio<bool>(exception);
        }
    }

    public async Task<Respuesta<List<TDisponibilidad>>> BuscarDisponibilidadAsync(
        TConsultaDisponibilidad datos,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        var parametros = new Dictionary<string, object?>
        {
            ["@Inicio"] = datos.FechaInicio.ToDateTime(TimeOnly.MinValue),
            ["@Fin"] = datos.FechaFin.ToDateTime(TimeOnly.MinValue),
            ["@Adultos"] = datos.CantidadAdultos,
            ["@Ninos"] = datos.CantidadNinos,
            ["@TipoHabitacionId"] = datos.TipoHabitacionId,
            ["@AmenidadId"] = datos.AmenidadId
        };

        try
        {
            await using var comando = PrepararComando("sp_BuscarDisponibilidad", parametros);
            bool cerrarConexion = await AbrirConexionAsync(cancellationToken);
            try
            {
                await using var lector = await comando.ExecuteReaderAsync(cancellationToken);
                var habitaciones = new List<TDisponibilidad>();
                long omitir = (long)(pagina - 1) * tamanoPagina;
                long posicion = 0;
                while (await lector.ReadAsync(cancellationToken))
                {
                    if (posicion++ < omitir)
                    {
                        continue;
                    }

                    habitaciones.Add(new TDisponibilidad
                    {
                        Id = lector.GetInt32(0),
                        Numero = lector.GetString(1),
                        Piso = lector.GetInt32(2),
                        Tipo = lector.GetString(3),
                        PrecioBasePorNoche = lector.GetDecimal(4),
                        Noches = lector.GetInt32(5),
                        CostoSinImpuestos = lector.GetDecimal(6),
                        Amenidades = lector.IsDBNull(7) ? null : lector.GetString(7),
                        FotoPrincipal = lector.IsDBNull(8) ? null : lector.GetString(8)
                    });

                    if (habitaciones.Count == tamanoPagina)
                    {
                        break;
                    }
                }

                return new Respuesta<List<TDisponibilidad>>
                {
                    Data = habitaciones
                };
            }
            finally
            {
                if (cerrarConexion)
                {
                    await _contexto.Database.CloseConnectionAsync();
                }
            }
        }
        catch (SqlException exception) when (EsErrorNegocio(exception))
        {
            return ErrorNegocio<List<TDisponibilidad>>(exception);
        }
    }

    private DbCommand PrepararComando(
        string procedimiento,
        IReadOnlyDictionary<string, object?> parametros)
    {
        var comando = _contexto.Database.GetDbConnection().CreateCommand();
        comando.CommandType = CommandType.StoredProcedure;
        comando.CommandText = "dbo." + procedimiento;
        comando.Transaction = _contexto.Database.CurrentTransaction?.GetDbTransaction();

        foreach (var dato in parametros)
        {
            var parametro = comando.CreateParameter();
            parametro.ParameterName = dato.Key;
            parametro.Value = dato.Value ?? DBNull.Value;
            if (dato.Value is DateTime)
            {
                parametro.DbType = DbType.Date;
            }
            comando.Parameters.Add(parametro);
        }

        return comando;
    }

    private async Task<bool> AbrirConexionAsync(CancellationToken cancellationToken)
    {
        if (_contexto.Database.GetDbConnection().State == ConnectionState.Open)
        {
            return false;
        }

        await _contexto.Database.OpenConnectionAsync(cancellationToken);
        return true;
    }

    private static bool EsErrorNegocio(SqlException exception)
    {
        return exception.Number is 50001 or 50002 or 50004;
    }

    private static Respuesta<T> ErrorNegocio<T>(SqlException exception)
    {
        int estado = exception.Number switch
        {
            50002 => 409,
            50004 => 404,
            _ => 400
        };

        return new Respuesta<T>
        {
            Success = false,
            EstadoHttp = estado,
            Error = exception.Number == 50004
                ? "No existe el registro solicitado."
                : "La operación no cumple las reglas del hospedaje. Revisá fechas, capacidad, estado y saldo."
        };
    }
}
