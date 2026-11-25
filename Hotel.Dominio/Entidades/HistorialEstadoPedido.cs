using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class HistorialEstadoPedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int EstadoPedidoId { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime Fecha { get; set; }

    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    public virtual PedidoRestaurante Pedido { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
