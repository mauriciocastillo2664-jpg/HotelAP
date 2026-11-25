using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Permiso
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Rol> Rols { get; set; } = new List<Rol>();
}
