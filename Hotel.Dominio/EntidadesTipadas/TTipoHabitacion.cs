namespace Hotel.Dominio.EntidadesTipadas;

public class TTipoHabitacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int CapacidadAdultos { get; set; }

    public int CapacidadNinos { get; set; }

    public decimal PrecioBasePorNoche { get; set; }

    public int PersonasIncluidas { get; set; }

    public decimal CargoPersonaAdicional { get; set; }

    public bool Activo { get; set; } = true;
}
