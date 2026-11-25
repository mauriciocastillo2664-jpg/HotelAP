namespace Hotel.Dominio.EntidadesTipadas;

public class TTemporada
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal PorcentajeRecargo { get; set; }

    public bool Activo { get; set; } = true;
}
