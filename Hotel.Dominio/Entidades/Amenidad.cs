using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Amenidad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Habitacion> Habitacions { get; set; } = new List<Habitacion>();
}
