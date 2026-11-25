namespace Hotel.Dominio.EntidadesTipadas;

public class TRefreshToken
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaRevocacion { get; set; }

    public int? TokenReemplazoId { get; set; }
}
