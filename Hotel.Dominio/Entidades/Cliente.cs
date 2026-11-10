using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Cliente
{
    public int Id { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string NumeroDocumento { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Nacionalidad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? Preferencias { get; set; }

    public decimal Descuento { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<PedidoRestaurante> PedidosRestaurantes { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
