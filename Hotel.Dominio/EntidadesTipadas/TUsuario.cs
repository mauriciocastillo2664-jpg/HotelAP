namespace Hotel.Dominio.EntidadesTipadas;

public class TUsuario
{
    public int Id { get; set; }

    public int TipoIdentificacionId { get; set; }

    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string NombreUsuario { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public int EstadoUsuarioId { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public int? ClienteId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
