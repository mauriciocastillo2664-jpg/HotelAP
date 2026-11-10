using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int PlatilloId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual PedidoRestaurante Pedido { get; set; } = null!;

    public virtual Platillo Platillo { get; set; } = null!;
}
