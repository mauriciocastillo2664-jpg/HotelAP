namespace Hotel.Dominio.EntidadesTipadas;

public class TModificarReservacion
{
    public int ReservacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int CantidadAdultos { get; set; }

    public int CantidadNinos { get; set; }
}
