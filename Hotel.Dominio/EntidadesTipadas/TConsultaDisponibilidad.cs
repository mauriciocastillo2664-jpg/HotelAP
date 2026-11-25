namespace Hotel.Dominio.EntidadesTipadas;

public class TConsultaDisponibilidad
{
    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int CantidadAdultos { get; set; } = 1;

    public int CantidadNinos { get; set; }

    public int? TipoHabitacionId { get; set; }

    public int? AmenidadId { get; set; }
}
