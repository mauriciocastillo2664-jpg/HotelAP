namespace Hotel.Dominio.EntidadesTipadas;

public class TFactura
{
    public int Id { get; set; }

    public string NumeroFactura { get; set; } = string.Empty;

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

}
