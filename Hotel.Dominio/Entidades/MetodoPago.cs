using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class MetodoPago
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool RequiereReferencia { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();
}
