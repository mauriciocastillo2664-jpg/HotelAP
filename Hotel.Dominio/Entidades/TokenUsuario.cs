using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class TokenUsuario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaUso { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
