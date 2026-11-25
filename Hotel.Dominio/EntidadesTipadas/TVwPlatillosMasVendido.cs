namespace Hotel.Dominio.EntidadesTipadas;

public class TVwPlatillosMasVendido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    public int? CantidadVendida { get; set; }

    public decimal? TotalVendido { get; set; }
}
