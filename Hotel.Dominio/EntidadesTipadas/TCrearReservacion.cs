namespace Hotel.Dominio.EntidadesTipadas;

public class TCrearReservacion
{
    public int ClienteId { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int CantidadAdultos { get; set; }

    public int CantidadNinos { get; set; }

    public string? CodigoDescuento { get; set; }

    public string? Observaciones { get; set; }
}
