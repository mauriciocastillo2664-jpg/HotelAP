namespace Hotel.Dominio.EntidadesTipadas;

public class TEstadoReservacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool OcupaHabitacion { get; set; }
}
