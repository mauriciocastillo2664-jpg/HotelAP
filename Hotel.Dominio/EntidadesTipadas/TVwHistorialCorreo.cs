namespace Hotel.Dominio.EntidadesTipadas;

public class TVwHistorialCorreo
{
    public int Id { get; set; }

    public string Direccion { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public string Remitente { get; set; } = string.Empty;

    public string Destinatario { get; set; } = string.Empty;

    public string Asunto { get; set; } = string.Empty;

    public string? Cliente { get; set; }

    public string? Reservacion { get; set; }

    public string? NumeroFactura { get; set; }

    public int? CorreoRespuestaId { get; set; }

    public int Intentos { get; set; }

    public string? MensajeError { get; set; }

    public string? EnviadoPor { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaEnvio { get; set; }
}
