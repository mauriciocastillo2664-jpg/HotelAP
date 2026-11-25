namespace Hotel.Dominio.EntidadesTipadas;

public class TVwReservacionesDetalle
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int? Noches { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Habitacion { get; set; } = string.Empty;

    public string TipoHabitacion { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public decimal AnticipoRequerido { get; set; }

    public decimal? Saldo { get; set; }
}
