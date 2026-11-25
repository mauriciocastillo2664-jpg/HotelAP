namespace Hotel.Dominio.EntidadesTipadas;

public class TRegistrarCorreo
{
    public string Direccion { get; set; } = string.Empty;

    public int TipoCorreoId { get; set; }

    public string Asunto { get; set; } = string.Empty;

    public string Cuerpo { get; set; } = string.Empty;

    public string? Destinatario { get; set; }

    public string? Remitente { get; set; }

    public int? ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? FacturaId { get; set; }

    public int? CorreoRespuestaId { get; set; }

}
