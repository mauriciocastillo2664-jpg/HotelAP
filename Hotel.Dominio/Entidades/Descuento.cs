using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Descuento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Codigo { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Valor { get; set; }

    public string AplicaA { get; set; } = null!;

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
}
