using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public int TipoIdentificacionId { get; set; }

    public string NumeroIdentificacion { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public int EstadoUsuarioId { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public int? ClienteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual ICollection<AjusteCuenta> AjustesCuentumAutorizadoPors { get; set; } = new List<AjusteCuenta>();

    public virtual ICollection<AjusteCuenta> AjustesCuentumUsuarios { get; set; } = new List<AjusteCuenta>();

    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    public virtual ICollection<CargoAdicional> CargosAdicionales { get; set; } = new List<CargoAdicional>();

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();

    public virtual ICollection<Estadia> EstadiaUsuarioCheckIns { get; set; } = new List<Estadia>();

    public virtual ICollection<Estadia> EstadiaUsuarioCheckOuts { get; set; } = new List<Estadia>();

    public virtual EstadoUsuario EstadoUsuario { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialEstadosHabitacion { get; set; } = new List<HistorialEstadoHabitacion>();

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual ICollection<HistorialReservacion> HistorialReservaciones { get; set; } = new List<HistorialReservacion>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();

    public virtual TipoIdentificacion TipoIdentificacion { get; set; } = null!;

    public virtual ICollection<TokenUsuario> TokensUsuario { get; set; } = new List<TokenUsuario>();

    public virtual ICollection<Rol> Rols { get; set; } = new List<Rol>();
}
