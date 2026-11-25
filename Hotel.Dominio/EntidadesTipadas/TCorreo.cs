namespace Hotel.Dominio.EntidadesTipadas;

public class TCorreo
{
    public int Id { get; set; }

    public string Direccion { get; set; } = string.Empty;

    public int TipoCorreoId { get; set; }

    public int EstadoCorreoId { get; set; }

    public string Remitente { get; set; } = string.Empty;

    public string Destinatario { get; set; } = string.Empty;

    public string Asunto { get; set; } = string.Empty;

    public string Cuerpo { get; set; } = string.Empty;

    public int? ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? FacturaId { get; set; }

    public int? CorreoRespuestaId { get; set; }

    public int Intentos { get; set; }

    public string? MensajeError { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaEnvio { get; set; }

}
