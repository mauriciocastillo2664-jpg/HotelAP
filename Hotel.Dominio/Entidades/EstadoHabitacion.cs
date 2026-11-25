using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class EstadoHabitacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool PermiteReservar { get; set; }

    public virtual ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialEstadoHabitacionEstadoAnteriors { get; set; } = new List<HistorialEstadoHabitacion>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialEstadoHabitacionEstadoNuevos { get; set; } = new List<HistorialEstadoHabitacion>();
}
