using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Correo
{
    public int Id { get; set; }

    public string Direccion { get; set; } = null!;

    public int TipoCorreoId { get; set; }

    public int EstadoCorreoId { get; set; }

    public string Remitente { get; set; } = null!;

    public string Destinatario { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string Cuerpo { get; set; } = null!;

    public int? ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public int? FacturaId { get; set; }

    public int? CorreoRespuestaId { get; set; }

    public int Intentos { get; set; }

    public string? MensajeError { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Correo? CorreoRespuesta { get; set; }

    public virtual EstadoCorreo EstadoCorreo { get; set; } = null!;

    public virtual Factura? Factura { get; set; }

    public virtual ICollection<Correo> InverseCorreoRespuesta { get; set; } = new List<Correo>();

    public virtual Reservacion? Reservacion { get; set; }

    public virtual TipoCorreo TipoCorreo { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
