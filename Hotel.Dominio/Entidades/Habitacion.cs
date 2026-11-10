using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Habitacion
{
    public int Id { get; set; }

    public string Numero { get; set; } = null!;

    public int Piso { get; set; }

    public int TipoHabitacionId { get; set; }

    public string EstadoOperativo { get; set; } = null!;

    public string? FotoUrl { get; set; }

    public bool Activo { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<ReservacionHabitacion> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacion>();

    public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;

    public virtual ICollection<Amenidad> Amenidads { get; set; } = new List<Amenidad>();
}
