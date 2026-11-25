namespace Hotel.Dominio.EntidadesTipadas;

public class TDisponibilidad
{
    public int Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public int Piso { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public decimal PrecioBasePorNoche { get; set; }

    public int Noches { get; set; }

    public decimal CostoSinImpuestos { get; set; }

    public string? Amenidades { get; set; }

    public string? FotoPrincipal { get; set; }
}
