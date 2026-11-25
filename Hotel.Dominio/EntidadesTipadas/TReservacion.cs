namespace Hotel.Dominio.EntidadesTipadas;

public class TReservacion
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public int ClienteId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int CantidadAdultos { get; set; }

    public int CantidadNinos { get; set; }

    public int EstadoReservacionId { get; set; }

    public int? DescuentoId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public decimal AnticipoRequerido { get; set; }

    public string? Observaciones { get; set; }

    public string? MotivoCancelacion { get; set; }

    public int? UsuarioCreacionId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
