using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Cliente
{
    public int Id { get; set; }

    public int TipoIdentificacionId { get; set; }

    public string NumeroIdentificacion { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Nacionalidad { get; set; }

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Preferencias { get; set; }

    public int? DescuentoId { get; set; }

    public int EstadoUsuarioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();

    public virtual Descuento? Descuento { get; set; }

    public virtual EstadoUsuario EstadoUsuario { get; set; } = null!;

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();

    public virtual TipoIdentificacion TipoIdentificacion { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
