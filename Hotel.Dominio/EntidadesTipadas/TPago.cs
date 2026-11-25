namespace Hotel.Dominio.EntidadesTipadas;

public class TPago
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

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

}
