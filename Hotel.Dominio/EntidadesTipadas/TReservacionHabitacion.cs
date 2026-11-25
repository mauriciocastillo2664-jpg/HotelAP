namespace Hotel.Dominio.EntidadesTipadas;

public class TReservacionHabitacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Subtotal { get; set; }
}
