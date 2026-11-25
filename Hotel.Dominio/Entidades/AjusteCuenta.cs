using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class AjusteCuenta
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public decimal Monto { get; set; }

    public string Motivo { get; set; } = null!;

    public int UsuarioId { get; set; }

    public int AutorizadoPorId { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Usuario AutorizadoPor { get; set; } = null!;

    public virtual Reservacion Reservacion { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
