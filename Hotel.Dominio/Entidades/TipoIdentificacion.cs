using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class TipoIdentificacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Formato { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<HuespedReservacion> HuespedesReservacion { get; set; } = new List<HuespedReservacion>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
