using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HuespedReservacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? NumeroDocumento { get; set; }

    public bool EsMenor { get; set; }

    public virtual Reservacion Reservacion { get; set; } = null!;
}
