using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? PedidoId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public decimal TotalPagado { get; set; }

    public DateTime FechaEmision { get; set; }

    public int? UsuarioId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();

    public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual PedidoRestaurante? Pedido { get; set; }

    public virtual Reservacion? Reservacion { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
