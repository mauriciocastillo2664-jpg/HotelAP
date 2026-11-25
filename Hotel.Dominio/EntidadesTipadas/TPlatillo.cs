namespace Hotel.Dominio.EntidadesTipadas;

public class TPlatillo
{
    public int Id { get; set; }

    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public string? ImagenUrl { get; set; }

    public int TiempoPreparacion { get; set; }

    public bool EsBebida { get; set; }

    public bool Disponible { get; set; }

    public bool Activo { get; set; } = true;
}
