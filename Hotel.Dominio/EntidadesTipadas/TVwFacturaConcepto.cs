namespace Hotel.Dominio.EntidadesTipadas;

public class TVwFacturaConcepto
{
    public int FacturaId { get; set; }

    public string NumeroFactura { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string? Reservacion { get; set; }

    public string TipoConcepto { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public decimal SubtotalFactura { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }
}
