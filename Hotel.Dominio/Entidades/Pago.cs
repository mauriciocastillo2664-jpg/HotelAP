using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Pago
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? PedidoId { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal TotalAbonado { get; set; }

    public decimal? Saldo { get; set; }

    public int EstadoPagoId { get; set; }

    public int? FacturaId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual EstadoPago EstadoPago { get; set; } = null!;

    public virtual Factura? Factura { get; set; }

    public virtual PedidoRestaurante? Pedido { get; set; }

    public virtual Reservacion? Reservacion { get; set; }
}
