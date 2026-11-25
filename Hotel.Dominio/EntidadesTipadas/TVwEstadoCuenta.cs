namespace Hotel.Dominio.EntidadesTipadas;

public class TVwEstadoCuenta
{
    public int PagoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string? Concepto { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal TotalAbonado { get; set; }

    public decimal? Saldo { get; set; }

    public string EstadoPago { get; set; } = string.Empty;

    public int? CantidadAbonos { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}
