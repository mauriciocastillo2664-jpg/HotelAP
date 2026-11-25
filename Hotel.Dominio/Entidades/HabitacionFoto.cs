using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HabitacionFoto
{
    public int Id { get; set; }

    public int HabitacionId { get; set; }

    public string Url { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EsPrincipal { get; set; }

    public int Orden { get; set; }

    public virtual Habitacion Habitacion { get; set; } = null!;
}
