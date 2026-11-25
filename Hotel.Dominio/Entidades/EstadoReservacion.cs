using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class EstadoReservacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool OcupaHabitacion { get; set; }

    public virtual ICollection<HistorialReservacion> HistorialReservacionEstadoAnteriors { get; set; } = new List<HistorialReservacion>();

    public virtual ICollection<HistorialReservacion> HistorialReservacionEstadoNuevos { get; set; } = new List<HistorialReservacion>();

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
}
