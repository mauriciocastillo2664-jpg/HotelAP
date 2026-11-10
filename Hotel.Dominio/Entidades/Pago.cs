using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Pago
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public decimal Monto { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? Referencia { get; set; }

    public DateTime FechaPago { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Reservacion Reservacion { get; set; } = null!;
}
