using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class EstadoPago
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
