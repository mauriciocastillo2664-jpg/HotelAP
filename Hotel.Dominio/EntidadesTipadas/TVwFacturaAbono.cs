namespace Hotel.Dominio.EntidadesTipadas;

public class TVwFacturaAbono
{
    public int FacturaId { get; set; }

    public string NumeroFactura { get; set; } = string.Empty;

    public int AbonoId { get; set; }

    public DateTime FechaAbono { get; set; }

    public string MetodoPago { get; set; } = string.Empty;

    public string? Referencia { get; set; }

    public string? UltimosDigitosTarjeta { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = string.Empty;

    public decimal TotalFactura { get; set; }

    public decimal TotalAbonado { get; set; }

    public decimal? Saldo { get; set; }

    public string EstadoPago { get; set; } = string.Empty;
}
