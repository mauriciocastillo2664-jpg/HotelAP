using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwHistorialCorreo
{
    public int Id { get; set; }

    public string Direccion { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Remitente { get; set; } = null!;

    public string Destinatario { get; set; } = null!;

    public string Asunto { get; set; } = null!;

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
