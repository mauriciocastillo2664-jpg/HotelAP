using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class ReservacionHabitacion
{
    public int Id { get; set; }

    public int ReservacionId { get; set; }

    public int HabitacionId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Reservacion Reservacion { get; set; } = null!;
}
