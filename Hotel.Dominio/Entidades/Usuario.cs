using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int? ClienteId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Estadia> EstadiaUsuarioCheckIns { get; set; } = new List<Estadia>();

    public virtual ICollection<Estadia> EstadiaUsuarioCheckOuts { get; set; } = new List<Estadia>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Rol> Rols { get; set; } = new List<Rol>();
}
