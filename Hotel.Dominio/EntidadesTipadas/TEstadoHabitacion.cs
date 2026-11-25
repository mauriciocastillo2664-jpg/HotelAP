namespace Hotel.Dominio.EntidadesTipadas;

public class TEstadoHabitacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public bool PermiteReservar { get; set; }
}
