namespace Hotel.Dominio.EntidadesTipadas;

public class TTokenUsuario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaUso { get; set; }
}
