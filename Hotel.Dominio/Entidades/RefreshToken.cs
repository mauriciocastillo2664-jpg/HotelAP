using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public DateTime? FechaRevocacion { get; set; }

    public int? TokenReemplazoId { get; set; }

    public virtual ICollection<RefreshToken> InverseTokenReemplazo { get; set; } = new List<RefreshToken>();

    public virtual RefreshToken? TokenReemplazo { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
