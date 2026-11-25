using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class VwUsuarioPermiso
{
    public int UsuarioId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public string Permiso { get; set; } = null!;
}
