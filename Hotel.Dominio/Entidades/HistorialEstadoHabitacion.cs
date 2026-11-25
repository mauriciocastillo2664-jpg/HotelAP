using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HistorialEstadoHabitacion
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public int? EstadoAnteriorId { get; set; }

    public int EstadoNuevoId { get; set; }

    public int? UsuarioId { get; set; }

    public string? Motivo { get; set; }

    public DateTime Fecha { get; set; }

    public virtual EstadoHabitacion? EstadoAnterior { get; set; }

    public virtual EstadoHabitacion EstadoNuevo { get; set; } = null!;

    public virtual Habitacion Habitacion { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
