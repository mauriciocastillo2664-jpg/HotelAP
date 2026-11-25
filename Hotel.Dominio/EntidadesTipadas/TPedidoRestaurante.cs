namespace Hotel.Dominio.EntidadesTipadas;

public class TPedidoRestaurante
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? HabitacionId { get; set; }

    public string Modalidad { get; set; } = string.Empty;

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

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

}
