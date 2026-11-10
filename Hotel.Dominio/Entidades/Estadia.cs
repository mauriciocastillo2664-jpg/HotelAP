using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Estadia
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public DateTime CheckInReal { get; set; }

    public DateTime? CheckOutReal { get; set; }

    public int UsuarioCheckInId { get; set; }

    public int? UsuarioCheckOutId { get; set; }

    public virtual Reservacion Reservacion { get; set; } = null!;

    public virtual Usuario UsuarioCheckIn { get; set; } = null!;

    public virtual Usuario? UsuarioCheckOut { get; set; }
}
