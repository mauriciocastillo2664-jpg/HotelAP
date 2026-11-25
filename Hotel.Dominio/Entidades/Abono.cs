using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Abono
{
    public int Id { get; set; }

    public int PagoId { get; set; }

    public int MetodoPagoId { get; set; }

    public decimal Monto { get; set; }

    public string? Referencia { get; set; }

    public string? UltimosDigitosTarjeta { get; set; }

    public string Estado { get; set; } = null!;

    public string? MotivoReverso { get; set; }

    public DateTime FechaAbono { get; set; }

    public int? UsuarioId { get; set; }

    public virtual MetodoPago MetodoPago { get; set; } = null!;

    public virtual Pago Pago { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
