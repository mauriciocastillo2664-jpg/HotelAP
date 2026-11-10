using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class PedidoRestaurante
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public int? ReservacionId { get; set; }

    public string Modalidad { get; set; } = null!;

    public DateTime FechaPedido { get; set; }

    public string EstadoPedido { get; set; } = null!;

    public string? InstruccionesEspeciales { get; set; }

    public decimal Total { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Reservacion? Reservacion { get; set; }
}
