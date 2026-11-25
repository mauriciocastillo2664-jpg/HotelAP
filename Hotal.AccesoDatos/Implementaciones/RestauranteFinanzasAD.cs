using Hotel.AccesoDatos.Contexto;
using Hotel.Dominio.EntidadesTipadas;
using Hotel.Dominio.InterfacesAD;
using Hotel.Utilitarios;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Hotel.AccesoDatos.Implementaciones;

public class RestauranteFinanzasAD : IRestauranteFinanzasAD
{
    private readonly string _cadenaConexion;

    public RestauranteFinanzasAD(HotelContext contexto)
    {
        _cadenaConexion = contexto.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Falta configurar la conexión de HotelBD.");
    }

    // Cada procedimiento es dueño de su transacción. No reutilizamos conexiones con seguimiento EF pendiente.
    private SqlConnection CrearConexion()
    {
        return new SqlConnection(_cadenaConexion);
    }

    private static Respuesta<T> ErrorDeNegocio<T>(int numero)
    {
        int estado = 400;
        string mensaje = "La operación no cumple las reglas del estado actual. Revisá los datos enviados.";

        if (numero == 50002)
        {
            estado = 409;
            mensaje = "La operación ya fue registrada o sus datos únicos están en uso.";
        }
        else if (numero == 50004)
        {
            estado = 404;
            mensaje = "No existe el registro solicitado.";
        }

        return new Respuesta<T>
        {
            Success = false,
            Error = mensaje,
            EstadoHttp = estado
        };
    }

    public async Task<Respuesta<int>> CrearPedidoAsync(
        TCrearPedido datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_CrearPedido";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@ClienteId", SqlDbType.Int).Value = datos.ClienteId;
        comando.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = datos.Modalidad;
        comando.Parameters.Add("@CargarAHabitacion", SqlDbType.Bit).Value = datos.CargarAHabitacion;
        comando.Parameters.Add("@FechaHoraDeseada", SqlDbType.DateTime).Value = (object?)datos.FechaHoraDeseada ?? DBNull.Value;
        comando.Parameters.Add("@Instrucciones", SqlDbType.NVarChar, 300).Value = (object?)datos.Instrucciones ?? DBNull.Value;
        comando.Parameters.Add("@CodigoDescuento", SqlDbType.NVarChar, 20).Value = (object?)datos.CodigoDescuento ?? DBNull.Value;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            var identificador = await comando.ExecuteScalarAsync(cancellationToken);
            return new Respuesta<int>
            {
                Data = Convert.ToInt32(identificador)
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<int>(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> AgregarPlatilloPedidoAsync(
        TAgregarPlatilloPedido datos,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_AgregarPlatilloPedido";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@PedidoId", SqlDbType.Int).Value = datos.PedidoId;
        comando.Parameters.Add("@PlatilloId", SqlDbType.Int).Value = datos.PlatilloId;
        comando.Parameters.Add("@Cantidad", SqlDbType.Int).Value = datos.Cantidad;

        try
        {
            await comando.ExecuteNonQueryAsync(cancellationToken);
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<bool>(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> CambiarEstadoPedidoAsync(
        TCambiarEstadoPedido datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_CambiarEstadoPedido";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@PedidoId", SqlDbType.Int).Value = datos.PedidoId;
        comando.Parameters.Add("@NuevoEstadoId", SqlDbType.Int).Value = datos.NuevoEstadoId;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            await comando.ExecuteNonQueryAsync(cancellationToken);
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<bool>(exception.Number);
        }
    }

    public async Task<Respuesta<int>> PagarPedidoAsync(
        TPagarPedido datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_PagarPedido";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@PedidoId", SqlDbType.Int).Value = datos.PedidoId;
        comando.Parameters.Add("@MetodoPagoId", SqlDbType.Int).Value = datos.MetodoPagoId;
        comando.Parameters.Add("@Referencia", SqlDbType.NVarChar, 100).Value = (object?)datos.Referencia ?? DBNull.Value;
        comando.Parameters.Add("@UltimosDigitos", SqlDbType.Char, 4).Value = (object?)datos.UltimosDigitos ?? DBNull.Value;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            var identificador = await comando.ExecuteScalarAsync(cancellationToken);
            return new Respuesta<int>
            {
                Data = Convert.ToInt32(identificador)
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<int>(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> RegistrarAbonoAsync(
        TRegistrarAbono datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_RegistrarAbono";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@ReservacionId", SqlDbType.Int).Value = datos.ReservacionId;
        var monto = comando.Parameters.Add("@Monto", SqlDbType.Decimal);
        monto.Precision = 10;
        monto.Scale = 2;
        monto.Value = datos.Monto;
        comando.Parameters.Add("@MetodoPagoId", SqlDbType.Int).Value = datos.MetodoPagoId;
        comando.Parameters.Add("@Referencia", SqlDbType.NVarChar, 100).Value = (object?)datos.Referencia ?? DBNull.Value;
        comando.Parameters.Add("@UltimosDigitos", SqlDbType.Char, 4).Value = (object?)datos.UltimosDigitos ?? DBNull.Value;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            await comando.ExecuteNonQueryAsync(cancellationToken);
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<bool>(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> ReversarAbonoAsync(
        TReversarAbono datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_ReversarAbono";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@AbonoId", SqlDbType.Int).Value = datos.AbonoId;
        comando.Parameters.Add("@Motivo", SqlDbType.NVarChar, 300).Value = datos.Motivo;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            await comando.ExecuteNonQueryAsync(cancellationToken);
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<bool>(exception.Number);
        }
    }

    public async Task<Respuesta<int>> RegistrarCorreoAsync(
        TRegistrarCorreo datos, int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_RegistrarCorreo";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@Direccion", SqlDbType.NVarChar, 10).Value = datos.Direccion;
        comando.Parameters.Add("@TipoCorreoId", SqlDbType.Int).Value = datos.TipoCorreoId;
        comando.Parameters.Add("@Asunto", SqlDbType.NVarChar, 200).Value = datos.Asunto;
        comando.Parameters.Add("@Cuerpo", SqlDbType.NVarChar, -1).Value = datos.Cuerpo;
        comando.Parameters.Add("@Destinatario", SqlDbType.NVarChar, 150).Value = (object?)datos.Destinatario ?? DBNull.Value;
        comando.Parameters.Add("@Remitente", SqlDbType.NVarChar, 150).Value = (object?)datos.Remitente ?? DBNull.Value;
        comando.Parameters.Add("@ClienteId", SqlDbType.Int).Value = (object?)datos.ClienteId ?? DBNull.Value;
        comando.Parameters.Add("@ReservacionId", SqlDbType.Int).Value = (object?)datos.ReservacionId ?? DBNull.Value;
        comando.Parameters.Add("@FacturaId", SqlDbType.Int).Value = (object?)datos.FacturaId ?? DBNull.Value;
        comando.Parameters.Add("@CorreoRespuestaId", SqlDbType.Int).Value = (object?)datos.CorreoRespuestaId ?? DBNull.Value;
        comando.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

        try
        {
            var identificador = await comando.ExecuteScalarAsync(cancellationToken);
            return new Respuesta<int>
            {
                Data = Convert.ToInt32(identificador)
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<int>(exception.Number);
        }
    }

    public async Task<Respuesta<bool>> ActualizarEstadoCorreoAsync(
        TActualizarEstadoCorreo datos,
        CancellationToken cancellationToken = default)
    {
        await using var conexion = CrearConexion();
        await conexion.OpenAsync(cancellationToken);
        await using var comando = conexion.CreateCommand();
        comando.CommandText = "dbo.sp_ActualizarEstadoCorreo";
        comando.CommandType = CommandType.StoredProcedure;

        comando.Parameters.Add("@CorreoId", SqlDbType.Int).Value = datos.CorreoId;
        comando.Parameters.Add("@Exito", SqlDbType.Bit).Value = datos.Exito;
        comando.Parameters.Add("@MensajeError", SqlDbType.NVarChar, 500).Value = (object?)datos.MensajeError ?? DBNull.Value;

        try
        {
            await comando.ExecuteNonQueryAsync(cancellationToken);
            return new Respuesta<bool>
            {
                Data = true
            };
        }
        catch (SqlException exception) when (exception.Number is >= 50001 and <= 50004)
        {
            return ErrorDeNegocio<bool>(exception.Number);
        }
    }
}
