using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class EstadoPedido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Orden { get; set; }

    public bool PermiteModificar { get; set; }

    public bool PermiteCancelar { get; set; }

    public bool EsFinal { get; set; }

    public virtual ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();
}
