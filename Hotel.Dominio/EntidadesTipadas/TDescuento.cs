namespace Hotel.Dominio.EntidadesTipadas;

public class TDescuento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Codigo { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string AplicaA { get; set; } = string.Empty;

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; } = true;
}
