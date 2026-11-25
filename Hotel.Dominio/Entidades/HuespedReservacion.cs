using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HuespedReservacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public int? TipoIdentificacionId { get; set; }

    public string? NumeroIdentificacion { get; set; }

    public bool EsMenor { get; set; }

    public bool EsTitular { get; set; }

    public virtual Reservacion Reservacion { get; set; } = null!;

    public virtual TipoIdentificacion? TipoIdentificacion { get; set; }
}
