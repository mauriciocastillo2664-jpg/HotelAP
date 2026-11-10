using System;
using System.Collections.Generic;

namespace Hotel.Dominio.Entidades;

public partial class Reservacion
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public int ClienteId { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public int CantidadAdultos { get; set; }

    public int CantidadNinos { get; set; }

    public string Estado { get; set; } = null!;

    public decimal CostoTotal { get; set; }

    public decimal AnticipoRequerido { get; set; }

    public string? Observaciones { get; set; }

    public string? MotivoCancelacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Estadia? Estadia { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<HuespedReservacion> HuespedesReservacions { get; set; } = new List<HuespedReservacion>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<PedidoRestaurante> PedidosRestaurantes { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<ReservacionHabitacion> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacion>();
}
