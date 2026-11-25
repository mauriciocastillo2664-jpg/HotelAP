using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwFacturaConcepto
{
    public int FacturaId { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public DateTime FechaEmision { get; set; }

    public string Cliente { get; set; } = null!;

    public string? Reservacion { get; set; }

    public string TipoConcepto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal SubtotalFactura { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }
}
