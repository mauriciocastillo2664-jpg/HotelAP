using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Habitacion
{
    public int Id { get; set; }

    public string Numero { get; set; } = null!;

    public int Piso { get; set; }

    public int TipoHabitacionId { get; set; }

    public int EstadoHabitacionId { get; set; }

    public bool Activo { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual EstadoHabitacion EstadoHabitacion { get; set; } = null!;

    public virtual ICollection<HabitacionFoto> HabitacionFotos { get; set; } = new List<HabitacionFoto>();

    public virtual ICollection<HistorialEstadoHabitacion> HistorialEstadosHabitacion { get; set; } = new List<HistorialEstadoHabitacion>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<ReservacionHabitacion> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacion>();

    public virtual TipoHabitacion TipoHabitacion { get; set; } = null!;

    public virtual ICollection<Amenidad> Amenidads { get; set; } = new List<Amenidad>();
}
