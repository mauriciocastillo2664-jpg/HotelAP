using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class EstadoCorreo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();
}
