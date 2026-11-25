using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class TipoCorreo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();
}
