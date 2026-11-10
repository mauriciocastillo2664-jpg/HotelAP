using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Factura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public int ReservacionId { get; set; }

    public DateTime FechaEmision { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public virtual Reservacion Reservacion { get; set; } = null!;
}
