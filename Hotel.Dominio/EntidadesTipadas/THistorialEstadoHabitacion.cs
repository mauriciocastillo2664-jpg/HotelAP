namespace Hotel.Dominio.EntidadesTipadas;

public class THistorialEstadoHabitacion
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public int? EstadoAnteriorId { get; set; }

    public int EstadoNuevoId { get; set; }

    public int? UsuarioId { get; set; }

    public string? Motivo { get; set; }

    public DateTime Fecha { get; set; }
}
