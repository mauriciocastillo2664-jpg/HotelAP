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

    public int EstadoReservacionId { get; set; }

    public int? DescuentoId { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuestos { get; set; }

    public decimal Total { get; set; }

    public decimal AnticipoRequerido { get; set; }

    public string? Observaciones { get; set; }

    public string? MotivoCancelacion { get; set; }

    public int? UsuarioCreacionId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<AjusteCuenta> AjustesCuenta { get; set; } = new List<AjusteCuenta>();

    public virtual ICollection<CargoAdicional> CargosAdicionales { get; set; } = new List<CargoAdicional>();

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<Correo> Correos { get; set; } = new List<Correo>();

    public virtual Descuento? DescuentoNavigation { get; set; }

    public virtual Estadia? Estadia { get; set; }

    public virtual EstadoReservacion EstadoReservacion { get; set; } = null!;

    public virtual Factura? Factura { get; set; }

    public virtual ICollection<HistorialReservacion> HistorialReservaciones { get; set; } = new List<HistorialReservacion>();

    public virtual ICollection<HuespedReservacion> HuespedesReservacion { get; set; } = new List<HuespedReservacion>();

    public virtual Pago? Pago { get; set; }

    public virtual ICollection<PedidoRestaurante> PedidosRestaurante { get; set; } = new List<PedidoRestaurante>();

    public virtual ICollection<ReservacionHabitacion> ReservacionHabitaciones { get; set; } = new List<ReservacionHabitacion>();

    public virtual Usuario? UsuarioCreacion { get; set; }
}
