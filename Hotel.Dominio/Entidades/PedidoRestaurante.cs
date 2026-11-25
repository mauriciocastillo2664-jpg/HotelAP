using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class PedidoRestaurante
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? HabitacionId { get; set; }

    public string Modalidad { get; set; } = null!;

    public bool CargoAhabitacion { get; set; }

    public int EstadoPedidoId { get; set; }

    public int? DescuentoId { get; set; }

    public DateTime FechaPedido { get; set; }

    public DateTime? FechaHoraDeseada { get; set; }

    public string? Instrucciones { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Recargo { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public int? UsuarioId { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Descuento? DescuentoNavigation { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    public virtual Factura? Factura { get; set; }

    public virtual Habitacion? Habitacion { get; set; }

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual Pago? Pago { get; set; }

    public virtual Reservacion? Reservacion { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
