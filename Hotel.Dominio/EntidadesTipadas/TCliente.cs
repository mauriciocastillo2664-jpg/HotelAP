namespace Hotel.Dominio.EntidadesTipadas;

public class TCliente
{
    public int Id { get; set; }

    public int TipoIdentificacionId { get; set; }

    public string NumeroIdentificacion { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string? Nacionalidad { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Preferencias { get; set; }

    public int? DescuentoId { get; set; }

    public int EstadoUsuarioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
