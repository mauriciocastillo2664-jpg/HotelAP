using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class ReservacionHabitacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int HabitacionId { get; set; }

    public decimal TarifaAplicada { get; set; }

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Reservacion Reservacion { get; set; } = null!;
}
